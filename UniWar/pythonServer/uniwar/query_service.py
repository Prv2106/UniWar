import bcrypt
import re


class LogInUserQuery:
    
    def __init__(self, username, password, conn):
        
        # verifica che la password non sia vuota
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
    
    def __init__(self, conn, username):
        self.conn = conn
        self.username = username
        self.username_check = """
            SELECT username 
            FROM Users 
            WHERE username = %s
        """
        
    
    @staticmethod
    def validate_username(username):
        username_regex = r"^[A-Z][a-zA-Z0-9_-]{4,}$" # Username che inizia con una lettera maiuscola e che deve avere almeno 4 altri caratteri tra quelli indicati nella classe []
        return re.match(username_regex, username) is not None
    


class GetDataQuery:
    
    def __init__(self, conn, game_id):
        
        self.get_data_query = """
        SELECT *
        FROM Data
        WHERE game_id = %s        
        """
        self.conn = conn
        self.game_id = game_id






class GetGamesQuery:
    def __init__(self,conn, username):
        
        self.conn = conn
        self.username = username
        self.get_games_query = """
        SELECT *
        FROM Games
        WHERE username = %s
        ORDER BY date DESC
        """
        
        


# Servizio che esegue le query
class QueryService:
    
    def handle_login_user_query(self, query: LogInUserQuery):
        with query.conn.cursor() as cursor:
            cursor.execute(query.get_password_query,(query.username,))
            result = cursor.fetchone()
            hashed_password_db = result[0] if result else None
        
        # Verifica dello username
        if hashed_password_db is None:
            raise ValueError("Username o Password non corretti")
    
        # Verifica la password
        if not bcrypt.checkpw(query.password.encode('utf-8'), hashed_password_db.encode('utf-8')):
            raise ValueError("Username o Password non corretti")
        
    def handle_username_check_query(self, query: UsernameCheckQuery):
        with query.conn.cursor() as cursor:
            cursor.execute(query.username_check,(query.username,))
            result = cursor.fetchone()
            if result is not None:
                raise ValueError("Username esistente")
            
            if not query.validate_username(query.username):
                raise ValueError("Username non valido: Deve iniziare con una lettera maiuscola e avere almeno 5 caratteri in totale")
            
            
    def handle_get_data_query(self, query: GetDataQuery):
            with query.conn.cursor() as cursor:
                cursor.execute(query.get_data_query,(query.game_id,))
                return cursor,cursor.fetchone()

    def handle_get_game_query(self, query: GetGamesQuery):
        with query.conn.cursor() as cursor:
            cursor.execute(query.get_games_query, (query.username,))
            result = cursor.fetchall()
            if result is None:
                raise ValueError("Non ci sono partite a te associate")
            return result