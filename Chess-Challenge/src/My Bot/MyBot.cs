using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        // System.Console.WriteLine(moves.Length);
        int bestEval = int.MinValue;
        Move moveToPlay = Move.NullMove;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };


        foreach(Move move in moves)
        {
            if(Evaluate(move) > bestEval)
            {
                moveToPlay = move;
                bestEval = Evaluate(move);
            }
        }

        
        return moveToPlay;


        int Evaluate(Move move)
        {
            int eval = 0;

            if(IsCheckmate(move))
            {
                // System.Console.WriteLine(board.PlyCount);
                return int.MaxValue;
            }

            if(IsCheck(move))
            {
                eval += 100;
            }

            if(move.IsCapture)
            {
                eval += pieceValues[(int)move.CapturePieceType];
            }

            if(move.IsPromotion)
            {
                eval += pieceValues[(int)move.PromotionPieceType];
            }

            if(move.IsCastles)
            {
                eval += 1000 / (board.PlyCount - 5);
            }

            if(board.SquareIsAttackedByOpponent(move.TargetSquare))
            {
                eval -= pieceValues[(int)move.MovePieceType];
            }

            if(board.SquareIsAttackedByOpponent(move.StartSquare))
            {
                eval += pieceValues[(int)move.MovePieceType];
            }
            

            if(board.GetPiece(move.StartSquare).IsPawn)
            {
                eval += 100 * (board.PlyCount - 20);
            }
            

            return eval;
        }



        bool IsCheckmate(Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }

        bool IsCheck(Move move)
        {
            board.MakeMove(move);
            bool isCheck = board.IsInCheck();
            board.UndoMove(move);
            return isCheck;
        }


    }
}