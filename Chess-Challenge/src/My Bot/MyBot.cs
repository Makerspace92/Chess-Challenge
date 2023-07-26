using ChessChallenge.API;
using System;
using System.Linq;
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
        Random rng = new Random();

        foreach(Move move in moves)
        {
            int moveEval = MiniMax(move, 4);
            
            if(moveEval > bestEval)
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
            int currentEval = -Search(Depth - 1);
            board.UndoMove(move); 

            int Search(uint depth)
            {
                if(depth == 0)
                {
                    return BoardEval();
                }

                Move[] searchMoves = board.GetLegalMoves();
                int searchBestEval = int.MinValue + 1;

                if(!searchMoves.Any())
                {
                    if(board.IsInCheckmate()){return int.MinValue + 1;}
                    return 0;
                }

                foreach(Move searchMove in searchMoves)
                {
                    board.MakeMove(searchMove);
                    int eval = -Search(depth - 1);
                    board.UndoMove(searchMove);
                    searchBestEval = eval > searchBestEval ? eval : searchBestEval;
                }
                return searchBestEval;
            }

            return currentEval;
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