using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
public class MyBot : IChessBot
{
    Dictionary<ulong, int> Transpositions = new Dictionary<ulong, int>();
    public Move Think(Board board, Timer timer)
    {
        Console.Clear();
        int ct = -1;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        Move[] moves = board.GetLegalMoves();
        Console.Write("Current evaluation: ", Console.ForegroundColor = ConsoleColor.DarkYellow);
        Console.WriteLine(BoardEval());
        int bestEval = int.MinValue + 1;
        Move moveToPlay = Move.NullMove;
        Random rng = new Random();

        foreach(Move move in moves)
        {
            int moveEval = MiniMax(move, 6);

            if(moveEval == int.MaxValue){return move;}
            
            if(moveEval >= bestEval + rng.Next(2))
            {
                moveToPlay = move;
                bestEval = moveEval;
            }
            
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(move);
            Console.Write("  Eval: ");
            Console.WriteLine(moveEval);
        }

        Console.Write("Total moves searched: ", Console.ForegroundColor = ConsoleColor.Cyan);
        Console.WriteLine(ct);
        return !moveToPlay.IsNull ? moveToPlay : moves[rng.Next(moves.Length)];
        
        int MiniMax(Move move, uint Depth)
        {
            board.MakeMove(move);

            ulong zobristKey = board.ZobristKey;

            int currentEval = -Search(Depth - 1, int.MinValue + 1, int.MaxValue);
            
            if(board.IsInCheck() && board.PlyCount >= 40){currentEval -= 100;}
            if(board.GameRepetitionHistory.Contains(zobristKey)){currentEval += 100;}

            board.UndoMove(move); 

            int Search(uint depth, int alpha, int beta)
            {
                if(depth == 0)
                {
                    // return BoardEval();
                    return SearchCaptures(alpha, beta);
                }

                Move[] searchMoves = board.GetLegalMoves();
                SortMoves(searchMoves);

                if(!searchMoves.Any())
                {
                    if(board.IsInCheckmate()){return int.MinValue + 1;}
                    if(board.IsDraw()){return 0;}
                }

                foreach(Move searchMove in searchMoves)
                {
                    board.MakeMove(searchMove);
                    int eval = -Search(depth - 1, -beta, -alpha);
                    board.UndoMove(searchMove);
                    if(eval >= beta){return beta;}
                    alpha = eval > alpha ? eval : alpha;
                }
                return alpha;
            }
            return currentEval;
        }

        int SearchCaptures(int alpha, int beta)
        {
            int eval = BoardEval();
            if(eval >= beta){return beta;}
            alpha = eval > alpha ? eval : alpha;

            Move[] captureMoves = board.GetLegalMoves(true);
            SortMoves(captureMoves);

            foreach(Move captureMove in captureMoves)
            {
                board.MakeMove(captureMove);
                eval = -SearchCaptures(-beta, -alpha);
                board.UndoMove(captureMove);

                if(eval >= beta){return beta;}
                alpha = eval > alpha ? eval : alpha;
            }
            return alpha;
        }

        void SortMoves(Move[] moveList)
        {
            Array.Sort(MoveEval(moveList), moveList);
        }
        
        int[] MoveEval(Move[] moves1)
        {   
            int[] evals = new int[moves1.Length];
            int i = 0;
            foreach(Move move1 in moves1)
            {
                int eval = 0;

                if(move1.IsCapture)
                {
                    eval += pieceValues[(int)move1.CapturePieceType] - pieceValues[(int)move1.MovePieceType];
                }

                if(move1.IsPromotion)
                {
                    eval += pieceValues[(int)move1.PromotionPieceType];
                }

                if(move1.IsCastles)
                {
                    eval += 1000 / (board.PlyCount - 5);
                }

                if(board.SquareIsAttackedByOpponent(move1.StartSquare))
                {
                    eval += pieceValues[(int)move1.MovePieceType];
                }

                if(board.SquareIsAttackedByOpponent(move1.TargetSquare))
                {
                    eval -= pieceValues[(int)move1.MovePieceType];
                }

                if((int)move1.MovePieceType == 1)
                {
                    eval += 50;
                }

                evals[i] = -eval;

                i++;
            }

            return evals;
        }

        int BoardEval()
        {
            if(Transpositions.ContainsKey(board.ZobristKey)){return Transpositions[board.ZobristKey];}

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

            if(Transpositions.Count < 256000){Transpositions.Add(board.ZobristKey, eval);}

            return eval;
        }
    }
}