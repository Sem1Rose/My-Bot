using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot : IChessBot
{
    #region Packed PSQ Table
    // Thanks https://github.com/Tyrant7/Easy-PST-Packer/tree/main and http://www.talkchess.com/forum3/viewtopic.php?f=2&t=68311&start=19
    decimal[] PackedPestoTables = {
            63746705523041458768562654720m, 71818693703096985528394040064m, 75532537544690978830456252672m, 75536154932036771593352371712m, 76774085526445040292133284352m, 3110608541636285947269332480m, 936945638387574698250991104m, 75531285965747665584902616832m,
            77047302762000299964198997571m, 3730792265775293618620982364m, 3121489077029470166123295018m, 3747712412930601838683035969m, 3763381335243474116535455791m, 8067176012614548496052660822m, 4977175895537975520060507415m, 2475894077091727551177487608m,
            2458978764687427073924784380m, 3718684080556872886692423941m, 4959037324412353051075877138m, 3135972447545098299460234261m, 4371494653131335197311645996m, 9624249097030609585804826662m, 9301461106541282841985626641m, 2793818196182115168911564530m,
            77683174186957799541255830262m, 4660418590176711545920359433m, 4971145620211324499469864196m, 5608211711321183125202150414m, 5617883191736004891949734160m, 7150801075091790966455611144m, 5619082524459738931006868492m, 649197923531967450704711664m,
            75809334407291469990832437230m, 78322691297526401047122740223m, 4348529951871323093202439165m, 4990460191572192980035045640m, 5597312470813537077508379404m, 4980755617409140165251173636m, 1890741055734852330174483975m, 76772801025035254361275759599m,
            75502243563200070682362835182m, 78896921543467230670583692029m, 2489164206166677455700101373m, 4338830174078735659125311481m, 4960199192571758553533648130m, 3420013420025511569771334658m, 1557077491473974933188251927m, 77376040767919248347203368440m,
            73949978050619586491881614568m, 77043619187199676893167803647m, 1212557245150259869494540530m, 3081561358716686153294085872m, 3392217589357453836837847030m, 1219782446916489227407330320m, 78580145051212187267589731866m, 75798434925965430405537592305m,
            68369566912511282590874449920m, 72396532057599326246617936384m, 75186737388538008131054524416m, 77027917484951889231108827392m, 73655004947793353634062267392m, 76417372019396591550492896512m, 74568981255592060493492515584m, 70529879645288096380279255040m,
    };
    #endregion

    #region variables

    struct TTEntry
    {
        public ulong hashCode;
        public int flag,
            depth,
            value;
        public Move bestMove;
    }
    TTEntry[] TT = new TTEntry[maxTTSize];
    Board board;
    Timer timer;
    Move BestMove;

    // Thanks http://www.talkchess.com/forum3/viewtopic.php?f=2&t=68311&start=19
    //                       P    N    B    R     Q   K
    short[] PieceValues = {  82, 281, 365, 477, 1025, 0, // Middlegame
                            100, 220, 297, 512, 1250, 0 }; // Endgame
    int[] hhScore;
    int[][] PSQTable;
    bool ContinueSearch => timer.MillisecondsElapsedThisTurn < Math.Clamp(timer.MillisecondsRemaining / 80, 700, 1000);

    const ulong maxTTSize = 0x2FFFFF;

    int infinity = 999999,
        maxHHandBFSize = 0x8FFFFF;

    #endregion

    #region Constructor

    // Thanks Again, Tyrant https://github.com/Tyrant7/Easy-PST-Packer/tree/main
    public MyBot()
    {
        PSQTable = PackedPestoTables.Select(packedTable =>
        {
            int pieceType = 0;
            return decimal.GetBits(packedTable).Take(3)
                .SelectMany(c => BitConverter.GetBytes(c)
                    .Select(square => (int)((sbyte)square * 1.461) + PieceValues[pieceType++]))
                .ToArray();
        }).ToArray();
    }
    #endregion

    #region Think
    public Move Think(Board _board, Timer _timer)
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine(PieceValues[i] / 100);
        }
        board = _board;
        timer = _timer;

        BestMove = board.GetLegalMoves()[0];
        if (timer.MillisecondsRemaining < 500)
            return BestMove;
        hhScore = new int[maxHHandBFSize];
        int maxDepth = 0;
        while (ContinueSearch)
            NegaMax(0, ++maxDepth, -infinity, infinity, 0);
        //Console.WriteLine($"FEN: {board.GetFenString()}\nPly {board.PlyCount}\tFinished at Depth: {maxDepth + (ContinueSearch ? -1 : 0)}\tin: {timer.MillisecondsElapsedThisTurn}\tBest Move: {BestMove}\n");
        return BestMove;
    }
    #endregion

    #region NegaMax Eval
    int NegaMax(int ply, int depth, int alpha, int beta, int numExtens)
    {
        ulong posHashKey = board.ZobristKey;
        bool qsearch = depth <= 0;
        var moves = board.GetLegalMoves(qsearch && !board.IsInCheck());
        Move bestMove = Move.NullMove,
             move;
        int bestEval = -infinity - 1;

        if (!ContinueSearch) return 0;
        if (ply != 0 && board.IsRepeatedPosition()) return 0;

        var ttEntry = TT[posHashKey % maxTTSize];
        if (ttEntry.hashCode == posHashKey && ttEntry.depth >= depth)
        {
            if (ttEntry.flag == 0) return ttEntry.value;
            else if (ttEntry.flag == 1) alpha = Math.Max(alpha, ttEntry.value);
            else if (ttEntry.flag == 2) beta = Math.Min(beta, ttEntry.value);
            if (alpha >= beta) return ttEntry.value;
        }

        if (qsearch)
        {
            bestEval = Evaluate();
            if (bestEval >= beta) return bestEval;
            alpha = Math.Max(alpha, bestEval);
        }

        var scores = new int[moves.Length];
        for (int i = 0; i < moves.Length; i++)
        {
            move = moves[i];
            board.MakeMove(move);
            if (board.IsRepeatedPosition())
            {
                board.UndoMove(move);
                scores[i] = -1;
                continue;
            }
            board.UndoMove(move);
            scores[i] = -(move == ttEntry.bestMove ? infinity : move.IsCapture ? 100 * (int)move.CapturePieceType - (int)move.MovePieceType : move.IsPromotion ? 1000 : move.IsCastles ? 200 : 2 * hhScore[move.GetHashCode() % maxHHandBFSize]);
        }
        Array.Sort(scores, moves);

        int alphaOrig = alpha;
        for (int i = 0; i < moves.Length; i++)
        {
            move = moves[i];
            board.MakeMove(move);
            int ext = numExtens < 5 && board.IsInCheck() ? 1 : 0;
            int score = -NegaMax(ply + 1, depth + ext - 1, -beta, -alpha, numExtens + ext);
            if (depth > 0 && !ContinueSearch)
                return bestEval;
            board.UndoMove(move);

            if (score > bestEval)
            {
                bestEval = score;
                bestMove = move;
                if (ply == 0)
                    BestMove = move;
                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                {
                    if (!move.IsCapture)
                        hhScore[move.GetHashCode() % maxHHandBFSize] += depth * depth;
                    break;
                }
            }
        }
        if (!qsearch && moves.Length == 0) return board.IsInCheck() ? -infinity + ply : 0;
        TT[posHashKey % maxTTSize] = new TTEntry { hashCode = posHashKey, flag = bestEval <= alphaOrig ? 2 : bestEval >= beta ? 1 : 0, depth = depth, value = bestEval, bestMove = bestMove };

        return bestEval;
    }
    int Evaluate()
    {
        int mg = 0, eg = 0, phase = 0;
        for (int square = 0; square < 64; ++square)
        {
            Piece piece = board.GetPiece(new Square(square));
            if (piece.IsNull) continue;
            int pieceIdx = (int)piece.PieceType - 1;
            mg += piece.IsWhite ? PSQTable[square ^ 56][pieceIdx] : -PSQTable[square][pieceIdx];
            eg += piece.IsWhite ? PSQTable[square ^ 56][pieceIdx + 6] : -PSQTable[square][pieceIdx + 6];
            phase += PieceValues[pieceIdx] / 100;
        }
        phase = Math.Min(phase, 56);

        return (board.IsWhiteToMove ? 1 : -1) * (mg * phase + eg * (56 - phase));
    }
    #endregion
}