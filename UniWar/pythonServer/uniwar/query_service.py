"""Query objects and query handlers for read operations.

Implements the query side of the service layer. Each query object encapsulates
a parameterized SQL statement, while ``QueryService`` executes it and validates
the result.
"""

import bcrypt
import re


class LogInUserQuery:
    """Query that retrieves the stored password hash for a username."""
    
    def __init__(self, username, password, conn):
        """Validates the credentials and prepares the lookup statement.

        Args:
            username (str): Username supplied by the client.
            password (str): Plain-text password supplied by the client.
            conn (pymysql.connections.Connection): Active database connection.

        Raises:
            ValueError: If the password or the username is empty.
        """
        if not password:
            print("Password non inserita", flush = True)
            raise ValueError("password non inserita")
        
        if not username:
           print("Username non inserito", flush = True)
           raise ValueError("Username non inserito")
       
       
        self.get_password_query = """
                SELECT pwd
                FROM Users
                WHERE username = %s;
            """
        
        self.username = username
        self.password = password
        self.conn = conn



class UsernameCheckQuery:
    """Query that checks username validity and uniqueness."""
    
    def __init__(self, conn, username):
        """Stores the connection and the username to validate.

        Args:
            conn (pymysql.connections.Connection): Active database connection.
            username (str): Username to validate.
        """
        self.conn = conn
        self.username = username
        self.username_check = """
            SELECT username 
            FROM Users 
            WHERE username = %s
        """
        
    
    @staticmethod
    def validate_username(username):
        """Checks that the username matches the required pattern.

        Args:
            username (str): Username to validate.

        Returns:
            bool: ``True`` when the username is valid.
        """
        username_regex = r"^[A-Z][a-zA-Z0-9_-]{4,}$"
        return re.match(username_regex, username) is not None
    


class GetDataQuery:
    """Query that retrieves the statistics row for a game."""
    
    def __init__(self, conn, game_id):
        """Stores the connection and the game identifier.

        Args:
            conn (pymysql.connections.Connection): Active database connection.
            game_id (int): Identifier of the target game.
        """
        self.get_data_query = """
        SELECT *
        FROM Data
        WHERE game_id = %s        
        """
        self.conn = conn
        self.game_id = game_id



class GetGamesQuery:
    """Query that retrieves the games played by a user, newest first."""

    def __init__(self, conn, username):
        """Stores the connection and the username.

        Args:
            conn (pymysql.connections.Connection): Active database connection.
            username (str): Owner of the games.
        """
        self.conn = conn
        self.username = username
        self.get_games_query = """
        SELECT *
        FROM Games
        WHERE username = %s
        ORDER BY date DESC
        """
        
        


class QueryService:
    """Executes query objects and validates their results."""
    
    def handle_login_user_query(self, query: LogInUserQuery):
        """Verifies the supplied password against the stored bcrypt hash.

        Args:
            query (LogInUserQuery): Query containing the credentials.

        Raises:
            ValueError: If the username does not exist or the password is wrong.
        """
        with query.conn.cursor() as cursor:
            cursor.execute(query.get_password_query,(query.username,))
            result = cursor.fetchone()
            hashed_password_db = result[0] if result else None
        
        # Verify the username
        if hashed_password_db is None:
            raise ValueError("Username o Password non corretti")
    
        # Verify the password
        if not bcrypt.checkpw(query.password.encode('utf-8'), hashed_password_db.encode('utf-8')):
            raise ValueError("Username o Password non corretti")
        
    def handle_username_check_query(self, query: UsernameCheckQuery):
        """Validates the username format and checks that it is available.

        Args:
            query (UsernameCheckQuery): Query containing the username.

        Raises:
            ValueError: If the username is invalid or already registered.
        """
        with query.conn.cursor() as cursor:
            cursor.execute(query.username_check,(query.username,))
            result = cursor.fetchone()
            
            if not query.validate_username(query.username):
                raise ValueError("Username non valido: Deve iniziare con una lettera maiuscola e avere almeno 5 caratteri in totale")
            
            if result is not None:
                raise ValueError("Username esistente")
            
    def handle_get_data_query(self, query: GetDataQuery):
        """Executes the statistics lookup and returns the cursor and row.

        Args:
            query (GetDataQuery): Query containing the game identifier.

        Returns:
            tuple: ``(cursor, row)`` where ``row`` is ``None`` when absent.
        """
        with query.conn.cursor() as cursor:
            cursor.execute(query.get_data_query,(query.game_id,))
            return cursor,cursor.fetchone()

    def handle_get_game_query(self, query: GetGamesQuery):
        """Executes the game-history lookup.

        Args:
            query (GetGamesQuery): Query containing the username.

        Returns:
            tuple: Rows describing the user's games.

        Raises:
            ValueError: If no games are associated with the username.
        """
        with query.conn.cursor() as cursor:
            cursor.execute(query.get_games_query, (query.username,))
            result = cursor.fetchall()
            if result is None:
                raise ValueError("Non ci sono partite a te associate")
            return result