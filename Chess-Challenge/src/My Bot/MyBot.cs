using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Console.Clear();
        int ct = -1;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        Move[] moves = board.GetLegalMoves();
        Console.Write("Current evaluation: ", Console.ForegroundColor = ConsoleColor.DarkYellow);
        Console.WriteLine(BoardEval());
        int bestEval = int.MinValue;
        Move moveToPlay = Move.NullMove;
        


        foreach(Move move in moves)
        {
            board.MakeMove(move);
            int currentEval = -MoveEval(5, int.MinValue, int.MaxValue);
            board.UndoMove(move);

            if(currentEval > bestEval)
            {
                moveToPlay = move;
                bestEval = currentEval;
            }
            Console.Write(move);
            Console.Write("  Eval: ", Console.ForegroundColor = ConsoleColor.DarkMagenta);
            Console.WriteLine(currentEval);
        }
        Console.Write("Total moves searched: ", Console.ForegroundColor = ConsoleColor.Cyan);
        Console.WriteLine(ct);
        return !moveToPlay.IsNull ? moveToPlay : moves[0];

        
        int MoveEval(uint deapth, int alpha, int beta)
        {
            if(deapth == 0)
            {
                return SearchCaptures(alpha, beta);
            }

            if(board.IsInCheckmate())
            {
                return int.MinValue;
            }

            foreach(Move move1 in board.GetLegalMoves())
            {
                board.MakeMove(move1);
                int eval1 = -MoveEval(deapth - 1, -beta, -alpha);
                board.UndoMove(move1);

                if(eval1 >= beta)
                {
                    return beta;
                }
                alpha = eval1 > alpha ? eval1 : alpha;
            }

            return alpha;
        }

        int SearchCaptures(int alpha, int beta)
        {
            int eval = BoardEval();
            if(eval >= beta)
            {
                return beta;
            }
            alpha = eval > alpha ? eval : alpha;

            foreach(Move capture in board.GetLegalMoves(true))
            {
                board.MakeMove(capture);
                eval = -SearchCaptures(-beta, -alpha);
                board.UndoMove(capture);

                if(eval >= beta)
                {
                    return beta;
                }
                alpha = eval > alpha ? eval : alpha;
            }
            return alpha;
        }



        int BoardEval()
        {
            ct++;
            int eval = 0;

            if(board.IsInCheck())
            {
                eval += 100;
            }

            // eval += (int)(board.GetPieceBitboard(PieceType.Pawn, board.IsWhiteToMove) / (ulong.MaxValue / 10));

            foreach(PieceList pieceList in board.GetAllPieceLists())
            {
                eval += pieceValues[(int)pieceList.TypeOfPieceInList] * pieceList.Count * (pieceList.IsWhitePieceList == board.IsWhiteToMove ? 1 : -1);
            }
            return eval;
        }
    }
}