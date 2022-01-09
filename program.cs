#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    //record Player( string Colour, string Symbol, string Name );
    
    // The `record` is a kind of automatic class in the sense that the compiler generates
    // the fields and constructor and some other things just from this one line.
    // There's a rationale for the capital letters on the variable names (later).
    // For now you can think of it as roughly equivalent to a nonstatic `class` with three
    // public fields and a three-parameter constructor which initializes the fields.
    // It would be like the following. The 'readonly' means you can't change the field value
    // after it is set by the constructor method.
    
    class Player
    {
        public readonly string Colour;
        public readonly string Symbol;
        public readonly string Name;
        
        public Player( string Colour, string Symbol, string Name )
        {
            this.Colour = Colour;
            this.Symbol = Symbol;
            this.Name = Name;
        }
    }
    
    static partial class Program
    {
        // Display common text for the top of the screen.
        
        static void Welcome( )
        {
			WriteLine( );
			WriteLine( " Welcome to Othello!" );
			WriteLine( );
        }
        
        // Collect a player name or default to form the player record.
        
        static Player NewPlayer( string colour, string symbol, string name )
        {
            return new Player( colour, symbol, name );
        }
        
        // Determine which player goes first or default.
        
        static int GetFirstTurn( Player[ ] players, int firstTurn )
        {
			string playerStartsFirst; 
			
			while( (firstTurn != 0) && (firstTurn != 1) ) // loop that ensures that player enters valid string
			{
				Write( " Choose who will play first [or <Enter> for black/X/{0}]: ", players[0].Name );
				playerStartsFirst = ReadLine( );
				
				if( playerStartsFirst.Length == 0 ) playerStartsFirst = "black";
				
				if( playerStartsFirst == "black" || playerStartsFirst == "X" || playerStartsFirst == players[0].Name ) firstTurn = 0;
				else if ( playerStartsFirst == "white" || playerStartsFirst == "O" || playerStartsFirst == players[1].Name ) firstTurn = 1; 
				
				//here firstTurn still == -1, which is an invalid index for array 
				else WriteLine( " Please enter a valid player name, corresponding colour, or symbol!" ); 
			}
			
			return firstTurn;
        }
        
        // Get a board size (between 4 and 26 and even) or default, for one direction.
        
        static int GetBoardSize( string direction, int size )
        {
			
			int directionSize;
			string response;
			
            Write(" Enter board {0} (4 to 26, even) [or <Enter> for 8]: ", direction);
            response = ReadLine( );
            
            if( response.Length == 0 ) return size;
            else directionSize = int.Parse( response );
		
            return directionSize;
        }
        
        // Get a move from a player.
        
        static string GetMove( Player player )
        {
			string choice; 
			
            WriteLine(" Turn is {0} disc ({1}) player: {2}", player.Colour, player.Symbol, player.Name );
            WriteLine(" Pick a cell by its row then column names (like 'bc') to play there.");
            WriteLine(" Use 'skip' to give up your turn. Use 'quit' to end game.");
            Write(" Enter your choice: ");
            choice = ReadLine( );
            
            return choice;
        }
        
        // Try to make a move. Return true if it worked.
        
        static bool TryMove( string[ , ] board, Player player, string move )
        {
			//if move is "skip" return true (no action needed)
			//if move length is not 2, return false
			//get first and last substrings of length 1 (x.Substring(s,1))
			//using IndexAtLetter( ), if index of either is -1, return false
			//otherwise save teh row/col as the move (in local variable)
			//if row or column too big for board, return false
			//if board occupied at that spot, return false
			//temporary: put the player symbol at that location
			//actual: call TryDirection eight times and keep track of whether any return true
			//if so return true 
			//otherwise return false
			
			string rowLetter, colLetter;
			int rowLetterIndex, colLetterIndex;
			bool recordResult = false; 
			 
			if( move == "skip" ) return true;
			if( move.Length != 2) return false;
			
			if( ( IndexAtLetter( move.Substring( 0, 1 ) ) == -1 ) || ( IndexAtLetter( move.Substring( 1 ) ) == -1 ) ) return false; 
			else
			{
				rowLetter = move.Substring( 0, 1);
				colLetter = move.Substring( 1 );
			}
			
			rowLetterIndex = IndexAtLetter( rowLetter );
			colLetterIndex = IndexAtLetter( colLetter );
			
			if( ( rowLetterIndex + 1 > board.GetLength( 0 ) ) || ( colLetterIndex + 1 > board.GetLength( 1 ) ) ) return false; 
			if( board[ rowLetterIndex, colLetterIndex ] != " ") return false; 
			
			
			board[ rowLetterIndex, colLetterIndex ] = player.Symbol; 
			
			for( int i = -1; i < 2; i++ )
			{
				for( int j = -1; j < 2; j++)
				{
					//runs 8 times instead of 9
					if( (i != 0) || (j != 0) ) 
					{ 
						if( TryDirection( board, player, rowLetterIndex, i, colLetterIndex, j ) ) recordResult = true;
					}
				}
			}
			
			if( recordResult ) return true; 
			
			board[ rowLetterIndex, colLetterIndex ] = " "; 
			
			return false;  
        }
        
        // Do the flips along a direction specified by the row and column delta for one step.
        
        static bool TryDirection( string[ , ] board, Player player, int moveRow, int deltaRow, int moveCol, int deltaCol )
        {
            int nextRow = moveRow + deltaRow; 
			if( nextRow < 0 || nextRow >= board.GetLength( 0 ) ) return false; 
			
			int nextCol = moveCol + deltaCol; 
			if( nextCol < 0 || nextCol >= board.GetLength( 1 ) ) return false; 
			
			if( board[ nextRow, nextCol ] == player.Symbol || board[ nextRow, nextCol ] == " ") return false; 
			
			bool validMove = true; 
			
			while (validMove)
			{
				// If can't move another step, set validMove false
				// Else if next step empty Set validMove = false
				// Else if next step is opponent, add to counter
				// Else // do some flips 
				// 	Starting back at move position 
				if( ( nextRow < 0 || nextRow >= board.GetLength( 0 ) ) || ( nextCol < 0 || nextCol >= board.GetLength( 1 ) ) ) validMove = false; 
				else if( board[ nextRow, nextCol ] == player.Symbol || board[ nextRow, nextCol ] == " " ) validMove = false;
				else if ( board[ nextRow, nextCol ] != player.Symbol ) 
				{
					nextRow += deltaRow;
					nextCol += deltaCol; 
				} 
			}
			
			if( ( nextRow < 0 || nextRow >= board.GetLength( 0 ) ) || ( nextCol < 0 || nextCol >= board.GetLength( 1 ) ) ) return false;
			if( board[ nextRow, nextCol ] != player.Symbol ) return false;
			
			
			//Do the flip
			if(deltaRow == 0)
			{
				for ( int c = moveCol + deltaCol; c != nextCol; c += deltaCol )
				{
					if( player.Symbol == "X" ) 
					{
						board[ moveRow, c ] = "X";
					}
					else if ( player.Symbol == "O" ) 
					{
						board[ moveRow, c ] = "O";
					}
				}
			}
			else
			{
				for ( int r = moveRow + deltaRow; r != nextRow; r += deltaRow )
				{
					//for no change in column, a special case
					if( deltaCol == 0 )
					{
						if( player.Symbol == "X" ) 
						{
							board[ r, moveCol ] = "X";
						}
						else if ( player.Symbol == "O" ) 
						{
							board[ r, moveCol ] = "O";
						}
					}
					else{
						for ( int c = moveCol + deltaCol; c != nextCol; c += deltaCol )
						{
						
							if( player.Symbol == "X" ) 
							{
								board[ r, c ] = "X";
							}
							else if ( player.Symbol == "O" ) 
							{
								board[ r, c ] = "O";
							}
						}
					}
				}
			}
			                                                       
            return true;      
        }
        
        // Count the discs to find the score for a player.
        
        static int GetScore( string[ , ] board, Player player )
        {
			int scoreCount = 0; 
			
			for( int r = 0; r < board.GetLength( 0 ); r ++ )
			{
				for( int c = 0; c < board.GetLength( 1 ); c++ )
				{
					if( board[ r, c ] == player.Symbol ) scoreCount++;
				}
			}
			
            return scoreCount;
        }
        
        // Display a line of scores for all players.
        
        static void DisplayScores( string[ , ] board, Player[ ] players )
        {
			int firstPlayerScore = GetScore( board, players[ 0 ] );
			int secondPlayerScore = GetScore( board, players[ 1 ] ); 
			
			WriteLine( );
			WriteLine( " Scores: {0} {1}, {2} {3}", players[ 0 ].Name, firstPlayerScore, players[ 1 ].Name, secondPlayerScore );
			WriteLine( );
        }
        
        // Display winner(s) and categorize their win over the defeated player(s).
        
        static void DisplayWinners( string[ , ] board, Player[ ] players )
        {
			int firstPlayerScore = GetScore( board, players[ 0 ] );
			int secondPlayerScore = GetScore( board, players[ 1 ] );
			int scoreDifference = 0;  
			string winner, looser; 
			string defeatLevel = " ";
			
			if( firstPlayerScore > secondPlayerScore )
			{
				scoreDifference = firstPlayerScore - secondPlayerScore;
				winner = players[ 0 ].Name; 
				looser = players[ 1 ].Name; 
			}
			else 
			{
				scoreDifference = secondPlayerScore - firstPlayerScore;
				winner = players[ 1 ].Name;
				looser = players[ 0 ].Name; 
			}
			
			if( scoreDifference == 0) 
			{
				WriteLine( );
				WriteLine(" Winners(s): {0} & {1}", players[ 0 ].Name, players[ 1 ].Name);
				WriteLine(" The game is a tie! We have 2 winners!");
				return; 
			}
			else if( scoreDifference >= 2 && scoreDifference <= 10 ) defeatLevel = "close";
			else if( scoreDifference >= 12 && scoreDifference <= 24 ) defeatLevel = "hot";
			else if( scoreDifference >= 26 && scoreDifference <= 38 ) defeatLevel = "fight";
			else if( scoreDifference >= 40 && scoreDifference <= 52 ) defeatLevel = "walkaway";
			else if( scoreDifference >= 54 && scoreDifference <= 60 ) defeatLevel = "perfect";
			
			WriteLine( );
			WriteLine( $" Winner(s): {winner}" );
			WriteLine( $" Defeated {looser} by {scoreDifference} in a {defeatLevel} game.");
			
			
        }
        
        static void Main( )
        {
            Welcome( );
            
            //Collect player names
            Write( " Type the balck disc (X) player name [or <Enter> for 'Black']: " );
            string blackName = ReadLine( );
            if( blackName.Length == 0 ) blackName = "Black";
            
            Write( " Type the white disc (O) player name [or <Enter> for 'White']: " );
            string whiteName = ReadLine( );
            if( whiteName.Length == 0 ) whiteName = "White";
            
            Player[ ] players = new Player[ ] 
            {
                NewPlayer( colour: "black", symbol: "X", name: blackName ),
                NewPlayer( colour: "white", symbol: "O", name: whiteName )
            };
            
            //Collect who start first
            int turn = GetFirstTurn( players, -1);
            
            //Collect board size
            int defaultSize = 8; 
            int rows = GetBoardSize( direction: "rows", defaultSize );
            int cols = GetBoardSize( direction: "columns", defaultSize );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            bool gameOver = false; 
            bool noMovesPossible = false;  
            int loopIteration = 0;
            int previousSkipIndex = 0;
            int skipIndex = 0;
                
            while( ! gameOver )
            {
				//loop iteration used to help determine consecutive 'skip' input 
				//assuming rational playing, 2 consecutive skip indicates no moves available. Game Over. 
				loopIteration++; 
				
				Console.Clear( );
                Welcome( );
                
                DisplayBoard( game ); 
                DisplayScores( game, players );
                
                string move = GetMove( players[ turn ] );
                
                if( move == "skip" ) 
                {
					previousSkipIndex = skipIndex; 
					skipIndex = loopIteration; 
					if( previousSkipIndex > 0 && skipIndex - previousSkipIndex == 1 ) noMovesPossible = true;  
				}
				
				if( noMovesPossible ) 
				{ 
					gameOver = true;
					Console.Clear( );
					Welcome( );
					DisplayBoard( game ); 
					DisplayScores( game, players );
					break; 
				}
				
                if( move == "quit" ) gameOver = true;
                else
                {
                    bool madeMove = TryMove( game, players[ turn ], move );
                    bool boardIsFilled = true; 
                    
                    if( madeMove ) turn = ( turn + 1 ) % players.Length;
                    else 
                    {
                        Write( " Your choice didn't work!" );
                        Write( " Press <Enter> to try again." );
                        ReadLine( ); 
                    }
                    
                    // if board is filled, game over
                    for( int r = 0; r < game.GetLength( 0 ); r ++ )
                    {
						for( int c = 0; c < game.GetLength( 1 ); c++ )
						{
							if( game[ r, c] == " ") boardIsFilled = false; 
						}
					} 
					
					if( boardIsFilled ) 
					{ 
						gameOver = true;
						Console.Clear( );
						Welcome( );
						DisplayBoard( game ); 
						DisplayScores( game, players );
					}
						
                } 
            }
            
            // Show the final results.
            
            DisplayWinners( game, players );
            WriteLine( );
        }
    }
}
