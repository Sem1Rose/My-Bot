using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

public class MyBot : IChessBot
{
	readonly int[] pieceValues = { 100, 300, 300, 450, 950, 10000 };

	// Thanks http://www.talkchess.com/forum3/viewtopic.php?f=2&t=68311&start=19
	//Piece-Square Table  [EG-MG][Piece Type][Rank * 8 + File]
	/*	int[][][] PSQTable = new int[2][][]{
		new int[6][] {
			new int[64] {
				0,   0,   0,   0,   0,   0,  0,   0,
				98, 134,  61,  95,  68, 126, 34, -11,
				-6,   7,  26,  31,  65,  56, 25, -20,
				-14,  13,   6,  21,  23,  12, 17, -23,
				-27,  -2,  -5,  12,  17,   6, 10, -25,
				-26,  -4,  -4, -10,   3,   3, 33, -12,
				-35,  -1, -20, -23, -15,  24, 38, -22,
				0,   0,   0,   0,   0,   0,  0,   0
			},
			new int[64] {
				-167, -89, -34, -49,  61, -97, -15, -107,
				-73, -41,  72,  36,  23,  62,   7,  -17,
				-47,  60,  37,  65,  84, 129,  73,   44,
				-9,  17,  19,  53,  37,  69,  18,   22,
				-13,   4,  16,  13,  28,  19,  21,   -8,
				-23,  -9,  12,  10,  19,  17,  25,  -16,
				-29, -53, -12,  -3,  -1,  18, -14,  -19,
				-105, -21, -58, -33, -17, -28, -19,  -23
			},
			new int[64] {
				-29,   4, -82, -37, -25, -42,   7,  -8,
				-26,  16, -18, -13,  30,  59,  18, -47,
				-16,  37,  43,  40,  35,  50,  37,  -2,
				-4,   5,  19,  50,  37,  37,   7,  -2,
				-6,  13,  13,  26,  34,  12,  10,   4,
				0,  15,  15,  15,  14,  27,  18,  10,
				4,  15,  16,   0,   7,  21,  33,   1,
				-33,  -3, -14, -21, -13, -12, -39, -21
			},
			new int[64] {
				32,  42,  32,  51, 63,  9,  31,  43,
				27,  32,  58,  62, 80, 67,  26,  44,
				-5,  19,  26,  36, 17, 45,  61,  16,
				-24, -11,   7,  26, 24, 35,  -8, -20,
				-36, -26, -12,  -1,  9, -7,   6, -23,
				-45, -25, -16, -17,  3,  0,  -5, -33,
				-44, -16, -20,  -9, -1, 11,  -6, -71,
				-19, -13,   1,  17, 16,  7, -37, -26
			},
			new int[64] {
				-28,   0,  29,  12,  59,  44,  43,  45,
				-24, -39,  -5,   1, -16,  57,  28,  54,
				-13, -17,   7,   8,  29,  56,  47,  57,
				-27, -27, -16, -16,  -1,  17,  -2,   1,
				-9, -26,  -9, -10,  -2,  -4,   3,  -3,
				-14,   2, -11,  -2,  -5,   2,  14,   5,
				-35,  -8,  11,   2,   8,  15,  -3,   1,
				-1, -18,  -9,  10, -15, -25, -31, -50
			},
			new int[64] {
				-65,  23,  16, -15, -56, -34,   2,  13,
				29,  -1, -20,  -7,  -8,  -4, -38, -29,
				-9,  24,   2, -16, -20,   6,  22, -22,
				-17, -20, -12, -27, -30, -25, -14, -36,
				-49,  -1, -27, -39, -46, -44, -33, -51,
				-14, -14, -22, -46, -44, -30, -15, -27,
				1,   7,  -8, -64, -43, -16,   9,   8,
				-15,  36,  12, -54,   8, -28,  24,  14
			},
		},
		new int[6][] {
			new int[64] {
				0,   0,   0,   0,   0,   0,   0,   0,
				178, 173, 158, 134, 147, 132, 165, 187,
				94, 100,  85,  67,  56,  53,  82,  84,
				32,  24,  13,   5,  -2,   4,  17,  17,
				13,   9,  -3,  -7,  -7,  -8,   3,  -1,
				4,   7,  -6,   1,   0,  -5,  -1,  -8,
				13,   8,   8,  10,  13,   0,   2,  -7,
				0,   0,   0,   0,   0,   0,   0,   0,
			},
			new int[64] {
				-58, -38, -13, -28, -31, -27, -63, -99,
				-25,  -8, -25,  -2,  -9, -25, -24, -52,
				-24, -20,  10,   9,  -1,  -9, -19, -41,
				-17,   3,  22,  22,  22,  11,   8, -18,
				-18,  -6,  16,  25,  16,  17,   4, -18,
				-23,  -3,  -1,  15,  10,  -3, -20, -22,
				-42, -20, -10,  -5,  -2, -20, -23, -44,
				-29, -51, -23, -15, -22, -18, -50, -64
			},
			new int[64] {
				-14, -21, -11,  -8, -7,  -9, -17, -24,
				-8,  -4,   7, -12, -3, -13,  -4, -14,
				2,  -8,   0,  -1, -2,   6,   0,   4,
				-3,   9,  12,   9, 14,  10,   3,   2,
				-6,   3,  13,  19,  7,  10,  -3,  -9,
				-12,  -3,   8,  10, 13,   3,  -7, -15,
				-14, -18,  -7,  -1,  4,  -9, -15, -27,
				-23,  -9, -23,  -5, -9, -16,  -5, -17
			},
			new int[64] {
				13, 10, 18, 15, 12,  12,   8,   5,
				11, 13, 13, 11, -3,   3,   8,   3,
				7,  7,  7,  5,  4,  -3,  -5,  -3,
				4,  3, 13,  1,  2,   1,  -1,   2,
				3,  5,  8,  4, -5,  -6,  -8, -11,
				-4,  0, -5, -1, -7, -12,  -8, -16,
				-6, -6,  0,  2, -9,  -9, -11,  -3,
				-9,  2,  3, -1, -5, -13,   4, -20
			},
			new int[64] {
				-9,  22,  22,  27,  27,  19,  10,  20,
				-17,  20,  32,  41,  58,  25,  30,   0,
				-20,   6,   9,  49,  47,  35,  19,   9,
				3,  22,  24,  45,  57,  40,  57,  36,
				-18,  28,  19,  47,  31,  34,  39,  23,
				-16, -27,  15,   6,   9,  17,  10,   5,
				-22, -23, -30, -16, -16, -23, -36, -32,
				-33, -28, -22, -43,  -5, -32, -20, -41
			},
			new int[64] {
				-74, -35, -18, -18, -11,  15,   4, -17,
				-12,  17,  14,  17,  17,  38,  23,  11,
				10,  17,  23,  15,  20,  45,  44,  13,
				-8,  22,  24,  27,  26,  33,  26,   3,
				-18,  -4,  21,  24,  27,  23,   9, -11,
				-19,  -3,  11,  21,  23,  16,   7,  -9,
				-27, -11,   4,  13,  14,   4,  -5, -17,
				-53, -34, -21, -11, -28, -14, -24, -43
			}
		}
	};*/

	// Thanks https://rustic-chess.org/search/ordering/mvv_lva.html
	// Most Valuable Victim, Least Valuable Attacker
	/*int[,] MVV_LVA = new int[5, 6]{
	    {15, 14, 13, 12, 11, 10}, // victim P, attacker P, N, B, R, Q, K
	    {25, 24, 23, 22, 21, 20}, // victim N, attacker P, N, B, R, Q, K
	    {35, 34, 33, 32, 31, 30}, // victim B, attacker P, N, B, R, Q, K
	    {45, 44, 43, 42, 41, 40}, // victim R, attacker P, N, B, R, Q, K
	    {55, 54, 53, 52, 51, 50}, // victim Q, attacker P, N, B, R, Q, K
	};*/

	Dictionary<ulong, (int value, int depth, Stack<Move> bestMoves)> TT = new();
    Stack<Move> BestMoves;
	Board board;
	Timer timer;
	DateTime IDStartTime;
	TimeSpan searchSpan;
    float endGameWeight;
	int[] piecesStartingFiles = new int[8]{1, 2, 0, 3, 6, 5, 7, 3};
	bool abortSearch = false;
    int infinity = 50000
		,maxDepth
		,smthng = 0
		,smthng2 = 0
		,smthng3 = 0;

	public Move Think(Board board, Timer timer)
	{
        this.board = board;
		this.timer = timer;

		int numEnemyPieces = 0;
		Enumerable.Range(1, 5).ToList().ForEach(x => numEnemyPieces += board.GetPieceList((PieceType)x, !board.IsWhiteToMove).Count);
		endGameWeight = Math.Clamp( Map(numEnemyPieces, 0, 12, 1.3f, 0f), 0, 1.3f);

		Console.WriteLine(board.ZobristKey);
		Console.WriteLine( "Ply: " + board.PlyCount + "\tEvaluation: " + Evaluate() * (board.IsWhiteToMove ? 1 : -1) + "\tEndgame weight: " + endGameWeight);

		float secs = Map(timer.MillisecondsRemaining, ChessChallenge.Application.Settings.GameDurationMilliseconds, 0, 8f, 2f);

		return IterativeDeepening(new TimeSpan(0, 0, 0, (int)Math.Floor(secs), (int)(secs % 1 * 100)));
	}

	Move IterativeDeepening(TimeSpan span)
	{
		IDStartTime = DateTime.Now;
		searchSpan = span;
		for (maxDepth = 1; maxDepth <= 4; maxDepth++)
		{
			TT.Clear();
            var (score, depthBestMoves) = NegaMax(maxDepth, -infinity, infinity);

			if(depthBestMoves.Count != 0)
				BestMoves = depthBestMoves;
			if (abortSearch)
				break;

			Console.WriteLine("Depth: " + maxDepth + "\t" + BestMoves.Peek() + "\t" + score + "\t" + timer.MillisecondsElapsedThisTurn + "\t" + smthng + "\t" + smthng2 + "\t" + smthng3);
			smthng = 0;
			smthng2 = 0;
			smthng3 = 0;
		}
		return BestMoves.Peek();
	}

    (int score, Stack<Move> bestMoves) NegaMax(int depth, int alpha, int beta)
	{
        if(DateTime.Now - IDStartTime >= searchSpan)
		{
            abortSearch = true;
            return (0, new Stack<Move>());
        }

        //#DEBUG
		smthng++;

        if (depth == 0 || board.IsInCheckmate())
            return (-Evaluate(), new Stack<Move>());// new SearchOut { score = -QuiesceSearch(alpha, beta) };

		Move[] moves = OrderBestMoves(board.GetLegalMoves().ToList(), maxDepth - depth);
		if (moves.Length == 0)
            return (board.IsInCheck() ? infinity : 0, new Stack<Move>()); //new SearchOut { score = infinity } : new SearchOut { score = 0 };

		int numChecks = 0,
			numCaptures = 0,
			bestEval = -infinity;
        Stack<Move> bestMoves = new();
		for (int i = 0; i < moves.Length; i++)
		{
			if (moves[i].IsCapture)
				numCaptures++;

			board.MakeMove(moves[i]);

			var (score, searchBestMoves) = NegaMax(depth - 1, -beta, -alpha);

            if (board.IsInCheck())
				numChecks++;
			
			if (board.GameRepetitionHistory.Any(x => x == board.ZobristKey) && depth == maxDepth)
				score -= infinity / 2 - 1;
			
			board.UndoMove(moves[i]);

			if (score > bestEval)
			{
				bestEval = score;
				if (searchBestMoves.Count == 0)
					bestMoves.Clear();
				else
					bestMoves = searchBestMoves;
				bestMoves.Push(moves[i]);
			}
            if (abortSearch)
                return (-bestEval, bestMoves);
            alpha = Math.Max(alpha, score);

			if (alpha >= beta)
				break;
		}

		if (numChecks >= 2 || numCaptures > 3 || endGameWeight >= .8f || depth <= 1)
			bestEval = Math.Max(bestEval, QuiesceSearch(alpha, beta, 5));

        return (-bestEval, bestMoves);
    }

    // Thanks https://www.chessprogramming.org/Quiescence_Search
    int QuiesceSearch(int alpha, int beta, int depth)
    {
		smthng2++;
        int stand_pat = Evaluate();
		if (depth == 0)
			return stand_pat;

        if (stand_pat >= beta)
            return beta;
        if (alpha < stand_pat)
            alpha = stand_pat;

		Move[] moves = OrderMoves(board.GetLegalMoves(true).ToList());
        for(int i = 0; i < moves.Length; i++)
        {
            board.MakeMove(moves[i]);
            int score = -QuiesceSearch(-beta, -alpha, depth - 1);
			board.UndoMove(moves[i]);

            if (score >= beta)
                return beta;
            if (score > alpha)
                alpha = score;
        }
        return alpha;
    }

    int Evaluate()
	{
		int whiteEval = 0, blackEval = 0;
		for (int i = 1; i < 6; i++)
		{
			whiteEval += EvaluatePieceList(board.GetPieceList((PieceType)i, true)); 
			blackEval += EvaluatePieceList(board.GetPieceList((PieceType)i, false));
		}
		return (whiteEval - blackEval) * (board.IsWhiteToMove ? 1 : -1);
	}

	int EvaluatePieceList(PieceList list)
	{
		if(list == null || list.Count == 0)
			return 0; 

		int listVal = 0;
		for (int i = 0, pieceVal = 0; i < list.Count; i++, listVal += pieceVal)
		{
			Piece piece = list.GetPiece(i);

			bool skipped = false;
			pieceVal = pieceValues[(int)piece.PieceType - 1];

			if (piece.Square.Rank == (piece.IsWhite ? 0 : 7)
				&& ((int)list.TypeOfPieceInList == 1 || piece.Square.File == piecesStartingFiles[(int)piece.PieceType - 2] || piece.Square.File == piecesStartingFiles[(int)piece.PieceType + 2]))
				pieceVal = (int)(pieceVal / 1.1f);

			if (board.IsWhiteToMove != piece.IsWhite)
			{
				skipped = true;
				board.ForceSkipTurn();
			}

			int numMoves = 0;
			Move[] legalMoves = board.GetLegalMoves().ToList().FindAll(x => x.MovePieceType == piece.PieceType && x.StartSquare == piece.Square).ToArray();
			foreach (var move in legalMoves) 
			{ 
				if (!board.SquareIsAttackedByOpponent(move.TargetSquare))
				{
					numMoves++;
					pieceVal += 5;
					if (piece.PieceType == PieceType.Pawn)
						pieceVal += 5;
				}
		}
            if (numMoves == 0)
                pieceVal = (int)(pieceVal / 1.1f);

            if (skipped)
                board.UndoSkipTurn();
        }
        return listVal;
    }

    // Thanks https://www.chessprogramming.org/Static_Exchange_Evaluation
    int EvaluateCaptures(Move parentMove, int depth)
    {
		smthng3++;
        if (!board.SquareIsAttackedByOpponent(parentMove.TargetSquare) || depth == 0)
            return 0;
        int value = 0;
        Move[] sqCaptures = board.GetLegalMoves(true).ToList().FindAll(x => x.TargetSquare == parentMove.TargetSquare).ToArray();

        Move move = Move.NullMove;
        for (int lowestPiece = infinity, i = 0; i < sqCaptures.Length; i++)
        {
            if ((int)sqCaptures[i].MovePieceType < lowestPiece)
            {
                lowestPiece = pieceValues[(int)sqCaptures[i].MovePieceType - 1] / 10;
                move = sqCaptures[i];
            }
        }

        board.MakeMove(move);
        value = (int)move.CapturePieceType * 10 + (6 - (int)move.MovePieceType) - EvaluateCaptures(move, depth - 1);
        board.UndoMove(move);

        return value;
    }

    // PV Ordering
    Move[] OrderBestMoves(List<Move> moves, int depth)
	{
		if (BestMoves != null && moves.Count != 0 && depth < BestMoves.Count)
		{
			Move bestMove = BestMoves.ToArray()[depth];
			if (moves.Contains(bestMove))
			{
				moves.RemoveAt(moves.IndexOf(bestMove));
				OrderMoves(moves);
				moves.Insert(0, bestMove);
				return moves.ToArray();
            }
		}
		return OrderMoves(moves);
	}

	Move[] OrderMoves(List<Move> moves)
	{
        int[] scores = new int[moves.Count];
        Enumerable.Range(0, moves.Count).ToList().ForEach(x => scores[x] = ScoreMove(moves[x]));
		var hell = scores.Zip(moves, (score, move) => new { Score = score, Move = move }).OrderByDescending(data => data.Score).ToList().Select(data => data.Move).ToArray();
        return hell;
    }

    int ScoreMove(Move move)
	{
		int score = 0;
		// Captures
		if (move.IsCapture)
			score += (int)move.CapturePieceType * 10 + (6 - (int)move.MovePieceType) - EvaluateCaptures(move, 5);//MVV_LVA[(int)move.CapturePieceType - 1, (int)move.MovePieceType - 1];

		// Promotion
		if (move.IsPromotion)
			score += 150;

		// Castling
		if (move.IsCastles)
			score += 50;

		// PSQ table
		// score += PSQTable[endGame >= .9f? 1 : 0][(int)move.MovePieceType - 1][move.TargetSquare.Index];

		board.MakeMove(move);
		
		Move[] kingLegalMoves = board.GetLegalMoves().ToList().FindAll(x => x.MovePieceType == PieceType.King).ToArray();
		score -= (int)(kingLegalMoves.Length * 5 * endGameWeight);
		for (int j = 0; j < kingLegalMoves.Length; j++)
		{
			Square opponentKSquare = board.GetKingSquare(!board.IsWhiteToMove);
			Square kSquare = board.GetKingSquare(board.IsWhiteToMove);

			// Minimize the distance between the kings
			score -= (int)((Math.Abs(kSquare.File - opponentKSquare.File) + Math.Abs(kSquare.Rank - opponentKSquare.Rank)) * 10 * endGameWeight);
			// Push the enemy king to the edge of the board
			score -= (int)((Math.Abs(kingLegalMoves[j].TargetSquare.File - (opponentKSquare.File < 4 ? 0 : 7)) + Math.Abs(kingLegalMoves[j].TargetSquare.Rank - (opponentKSquare.Rank < 4 ? 0 : 7))) * 100 * endGameWeight);
		}

		// Rate move based on the value of the undefended pieces attacked
		board.ForceSkipTurn();
		Move[] movedPieceLegalMoves = board.GetLegalMoves(true).ToList().FindAll(x => x.MovePieceType == move.MovePieceType && x.StartSquare == move.TargetSquare).ToArray();
		foreach (var pieceMove in movedPieceLegalMoves)
			if (!board.SquareIsAttackedByOpponent(pieceMove.TargetSquare))
				score += (int)(pieceValues[(int)pieceMove.CapturePieceType - 1] / 20f);
		board.UndoSkipTurn();

		// Encourage moves that lead to checkmate, discourage stalemate and draw
		if (board.IsInCheckmate())
			score = infinity;
		else if (board.IsDraw() || board.IsInStalemate())
			score = -infinity;
		board.UndoMove(move);

		return score;
	}    

    static float Map(float s, float a1, float a2, float b1, float b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	// float inverseLerp(float a, float b, float v) => (v - a) / (b - a);
}