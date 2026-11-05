"""Command objects and command handlers for write operations.

Implements the command side of the CQRS-style service layer: each command
encapsulates a parameterized SQL statement, while ``CommandService`` executes it
against a MySQL connection.
"""

import bcrypt
import re
from uniwar import query_service


class RegisterUserCommand:
    """Command that registers a user with a bcrypt-hashed password."""

    def __init__(self, username, password, conn):
        """Validates credentials and prepares the registration statement.

        Args:
            username (str): Username to register.
            password (str): Plain-text password to hash.
            conn (pymysql.connections.Connection): Active database connection.

        Raises:
            ValueError: If the password is empty or the username is invalid.
        """
        if not password:
            raise ValueError("password non inserita")
        
        if not RegisterUserCommand.validate_username(username):
           raise ValueError("Username non valido: Deve iniziare con una lettera maiuscola e avere almeno 5 caratteri in totale")
         
        hashed_password = bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt())
        hashed_password_str = hashed_password.decode('utf-8')  # Store the hash as text
        
        self.register_user_command = """
        INSERT INTO Users (username, pwd)
        VALUES (%s, %s);
        """
        self.username = username
        self.hashed_pwd = hashed_password_str
        self.conn = conn


    @staticmethod
    def validate_username(username):
        """Checks that the username starts with an uppercase letter.

        The username must start with ``A-Z`` followed by at least four
        characters from ``[a-zA-Z0-9_-]``.

        Args:
            username (str): Username to validate.

        Returns:
            bool: ``True`` when the username matches the required pattern.
        """
        username_regex = r"^[A-Z][a-zA-Z0-9_-]{4,}$"
        return re.match(username_regex, username) is not None
    
    
    
class InsertDataCommand:
    """Command that upserts per-game statistics after a battle phase.

    When a row already exists for the game, counters are accumulated and the
    row is updated; otherwise a new row is inserted.
    """
   
    def __init__(self, conn, **kwargs):
        """Builds the INSERT or UPDATE statement from the supplied metrics.

        Args:
            conn (pymysql.connections.Connection): Active database connection.
            **kwargs: Battle metrics, game/round identifiers, ownership lists,
                and turn metadata.
        """
        self.conn = conn
        game_id =kwargs['game_id']
        round_id = kwargs['round_id']
        user_owned_territories = kwargs['user_owned_territories']
        user_owned_tanks = kwargs['user_owned_tanks']        
        cpu_owned_territories = kwargs['cpu_owned_territories']        
        cpu_owned_tanks = kwargs['cpu_owned_tanks']        
        user_owned_territories_list = kwargs['user_owned_territories_list']        
        cpu_owned_territories_list = kwargs['cpu_owned_territories_list']
        
        print(f"round_id ricevuto da request = {round_id}", flush=True)
        

        service = query_service.QueryService()
        _,result =  service.handle_get_data_query(query_service.GetDataQuery(conn,kwargs['game_id']))

        if result is not None:
            (_,round_id,turn_completed,_,_,_,_,_,_,_,user_tanks_lost,cpu_tanks_lost,user_tanks_lost_attacking,cpu_tanks_lost_attacking,
            user_tanks_lost_defending,cpu_tanks_lost_defending,user_perfect_defenses,cpu_perfect_defenses,
            user_territories_lost,cpu_territories_lost, is_user_turn) = result
            
            print(f"round_id recuperato dal db = {round_id}", flush=True)
            if round_id == kwargs['round_id']:
                if is_user_turn != kwargs['user_turn']:  # Turn changed within the same round
                    turn_completed += 1
            else:
                turn_completed = 1
                
            print(f"turn_completed = {turn_completed}", flush=True)
    
            
            user_tanks_lost += kwargs['user_tanks_lost']
            user_tanks_lost_attacking += kwargs['user_tanks_lost_attacking']
            user_tanks_lost_defending += kwargs['user_tanks_lost_defending']
            cpu_tanks_lost += kwargs['cpu_tanks_lost'] 
            cpu_tanks_lost_attacking += kwargs['cpu_tanks_lost_attacking']
            cpu_tanks_lost_defending += kwargs['cpu_tanks_lost_defending']
            user_territories_lost += kwargs['user_territories_lost']
            cpu_territories_lost += kwargs['cpu_territories_lost']
            user_perfect_defenses += kwargs['user_perfect_defenses']
            cpu_perfect_defenses += kwargs['cpu_perfect_defenses']
            
            self.insert_command_query = """
                UPDATE Data
                SET 
                    user_tanks_lost = %s, 
                    user_tanks_lost_attacking = %s, 
                    user_tanks_lost_defending = %s, 
                    cpu_tanks_lost = %s, 
                    cpu_tanks_lost_attacking = %s, 
                    cpu_tanks_lost_defending = %s, 
                    user_territories_lost = %s, 
                    cpu_territories_lost = %s, 
                    user_perfect_defenses = %s, 
                    cpu_perfect_defenses = %s, 
                    turn_completed = %s, 
                    round_id = %s, 
                    user_owned_territories = %s, 
                    user_owned_tanks = %s, 
                    cpu_owned_territories = %s, 
                    cpu_owned_tanks = %s, 
                    user_owned_territories_list = %s, 
                    cpu_owned_territories_list = %s,
                    is_user_turn = %s
                WHERE game_id = %s;
            """
            
            self.values_tuple = (
                user_tanks_lost,
                user_tanks_lost_attacking,
                user_tanks_lost_defending,
                cpu_tanks_lost,
                cpu_tanks_lost_attacking,
                cpu_tanks_lost_defending,
                user_territories_lost,
                cpu_territories_lost,
                user_perfect_defenses,
                cpu_perfect_defenses,
                turn_completed,
                kwargs['round_id'],
                user_owned_territories,
                user_owned_tanks,
                cpu_owned_territories,
                cpu_owned_tanks,
                user_owned_territories_list,
                cpu_owned_territories_list,
                kwargs['user_turn'],
                game_id
            )


        else:
            user_tanks_lost = kwargs['user_tanks_lost']
            user_tanks_lost_attacking = kwargs['user_tanks_lost_attacking']
            user_tanks_lost_defending = kwargs['user_tanks_lost_defending']
            cpu_tanks_lost = kwargs['cpu_tanks_lost'] 
            cpu_tanks_lost_attacking = kwargs['cpu_tanks_lost_attacking']
            cpu_tanks_lost_defending = kwargs['cpu_tanks_lost_defending']
            user_territories_lost = kwargs['user_territories_lost']
            cpu_territories_lost = kwargs['cpu_territories_lost']
            user_perfect_defenses = kwargs['user_perfect_defenses']
            cpu_perfect_defenses = kwargs['cpu_perfect_defenses']
            turn_completed = 1
            print(f"turn_completed = {turn_completed}", flush=True)

            
            self.insert_command_query = """
                INSERT INTO Data (
                    user_tanks_lost, 
                    user_tanks_lost_attacking, 
                    user_tanks_lost_defending, 
                    cpu_tanks_lost, 
                    cpu_tanks_lost_attacking, 
                    cpu_tanks_lost_defending, 
                    user_territories_lost, 
                    cpu_territories_lost, 
                    user_perfect_defenses, 
                    cpu_perfect_defenses, 
                    turn_completed, 
                    game_id, 
                    round_id, 
                    user_owned_territories, 
                    user_owned_tanks, 
                    cpu_owned_territories, 
                    cpu_owned_tanks, 
                    user_owned_territories_list, 
                    cpu_owned_territories_list,
                    is_user_turn
                ) 
                VALUES (
                    %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s
                );
            """
            self.values_tuple = (
                user_tanks_lost,
                user_tanks_lost_attacking,
                user_tanks_lost_defending,
                cpu_tanks_lost,
                cpu_tanks_lost_attacking,
                cpu_tanks_lost_defending,
                user_territories_lost,
                cpu_territories_lost,
                user_perfect_defenses,
                cpu_perfect_defenses,
                turn_completed,
                game_id,
                kwargs['round_id'],
                user_owned_territories,
                user_owned_tanks,
                cpu_owned_territories,
                cpu_owned_tanks,
                user_owned_territories_list,
                cpu_owned_territories_list,
                kwargs['user_turn'] 
            )


    
class InsertGameCommand:
    """Command that creates a new game record."""

    def __init__(self, conn, username, date):
        """Stores the connection and the values used by the INSERT statement.

        Args:
            conn (pymysql.connections.Connection): Active database connection.
            username (str): Owner of the game.
            date (str): Formatted creation timestamp.
        """
        self.conn = conn
        self.username = username
        self.date = date
        self.insert_game_query = """
            INSERT INTO Games (username, date) 
            VALUES (%s, %s)
        """

class EndGameCommand:
    """Command that stores the final state of a game."""

    def __init__(self, conn, game_id, is_win):
        """Stores the connection and the values used by the UPDATE statement.

        Args:
            conn (pymysql.connections.Connection): Active database connection.
            game_id (int): Identifier of the game to update.
            is_win (bool): Whether the user won the game.
        """
        self.conn = conn
        self.game_id = game_id
        self.is_win = 1 if is_win else 0
        self.end_game_query = """
        UPDATE Games
        SET state = %s
        WHERE id = %s
        """


class CommandService:
    """Executes command objects against the database."""
    
    def handle_register_user(self, command: RegisterUserCommand):
        """Executes a user registration command and commits the transaction.

        Args:
            command (RegisterUserCommand): Command to execute.
        """
        with command.conn.cursor() as cursor:
            cursor.execute(command.register_user_command, (command.username, command.hashed_pwd,))
            command.conn.commit()
            
    def handle_insert_data_command(self, command: InsertDataCommand):
        """Executes a statistics upsert command and commits the transaction.

        Args:
            command (InsertDataCommand): Command to execute.

        Raises:
            ValueError: If the statement affected no rows.
        """
        with command.conn.cursor() as cursor:
            cursor.execute(command.insert_command_query, command.values_tuple)
            if cursor.rowcount == 0:
                raise ValueError("Si è verificato un problema durante l'aggiornamento dei dati")
            command.conn.commit()
    
    def handle_insert_game_command(self, command: InsertGameCommand) -> int:
        """Inserts a new game and returns its generated identifier.

        Args:
            command (InsertGameCommand): Command to execute.

        Returns:
            int: Identifier assigned to the new game.

        Raises:
            ValueError: If the statement affected no rows.
        """
        with command.conn.cursor() as cursor:
            cursor.execute(command.insert_game_query, (command.username, command.date))
            if cursor.rowcount == 0:
                raise ValueError("Si è verificato un problema durante l'inserimento della partita")
            
            command.conn.commit()
            game_id = cursor.lastrowid  # Identifier of the last inserted row
            return game_id
            
            
    def handle_end_game_command(self, command: EndGameCommand):
        """Updates the final state of a game and commits the transaction.

        Args:
            command (EndGameCommand): Command to execute.
        """
        with command.conn.cursor() as cursor:
            cursor.execute(command.end_game_query, (command.is_win, command.game_id))
            command.conn.commit()
        