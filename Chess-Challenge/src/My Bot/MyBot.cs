using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        int ct = -1;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        Move[] moves = board.GetLegalMoves();
        System.Console.WriteLine(BoardEval());
        int bestEval = int.MinValue;
        Move moveToPlay = Move.NullMove;
        


        foreach(Move move in moves)
        {
            board.MakeMove(move);
            int currentEval = MoveEval();
            board.UndoMove(move);

            if(currentEval > bestEval)
            {
                moveToPlay = move;
                bestEval = currentEval;
            }
            // System.Console.Write(move);
            // System.Console.Write("  Eval: ");
            // System.Console.WriteLine(currentEval);
        }

         System.Console.WriteLine(ct);
        return moveToPlay;

        
        int MoveEval(uint deapth=4)
        {
            if(deapth == 0)
            {
                return BoardEval();
            }

            if(board.IsInCheckmate())
            {
                return int.MinValue;
            }

            int bestBoardEval = int.MinValue;
            foreach(Move move1 in board.GetLegalMoves())
            {
                board.MakeMove(move1);
                int eval1 = MoveEval(deapth - 1);
                bestBoardEval = eval1 > bestBoardEval ? eval1 : bestBoardEval;
                board.UndoMove(move1);
            }

            return bestBoardEval;
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