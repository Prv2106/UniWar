import bcrypt
import re
import query_service


# Command per la registrazione dell'utente
class RegisterUserCommand:
    def __init__(self, username, password, conn):
        
        if not password:
            raise ValueError("password non inserita")
        
        if not RegisterUserCommand.validate_username(username):
           raise ValueError("Username non valido: Deve iniziare con una lettera maiuscola e avere almeno 5 caratteri in totale")
         
        hashed_password = bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt())
        hashed_password_str = hashed_password.decode('utf-8')  # Convertiamo l'hash in stringa per il database
        
        self.register_user_command = """
        INSERT INTO Users (username, pwd)
        VALUES (%s, %s);
        """
        self.username = username
        self.hashed_pwd = hashed_password_str
        self.conn = conn


    @staticmethod
    def validate_username(username):
        username_regex = r"^[A-Z][a-zA-Z0-9_-]{4,}$" # Username che inizia con una lettera maiuscola e che deve avere almeno 1 altro carattere tra quelli indicati nella classe []
        return re.match(username_regex, username) is not None
    
    
    
class InsertDataCommand:
    def __init__(self, conn, **kwargs):
        
        self.conn = conn
        game_id =kwargs['game_id']
        round_id = kwargs['round_id']
        user_owned_territories = kwargs['user_owned_territories']
        user_owned_tanks = kwargs['user_owned_tanks']        
        cpu_owned_territories = kwargs['cpu_owned_territories']        
        cpu_owned_tanks = kwargs['cpu_owned_tanks']        
        user_owned_territories_list = kwargs['user_owned_territories_list']        
        cpu_owned_territories_list = kwargs['cpu_owned_territories_list']        
        user_win = kwargs['user_win'] 
           
        service = query_service.QueryService()
        result =  service.handle_get_data_query(query_service.GetDataQuery(conn,kwargs['game_id']))
        
        if result is not None:
            (_,_,turn_completed,_,_,_,_,_,_,_,user_tanks_lost,cpu_tanks_lost,user_tanks_lost_attacking,cpu_tanks_lost_attacking,
            user_tanks_lost_defending,cpu_tanks_lost_defending,user_perfect_defenses,cpu_perfect_defenses,
            user_territories_lost,cpu_territories_lost) = result
            
            turn_completed = 1 if turn_completed == 0 else 2
            
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
                    user_win = %s
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
                round_id,
                user_owned_territories,
                user_owned_tanks,
                cpu_owned_territories,
                cpu_owned_tanks,
                user_owned_territories_list,
                cpu_owned_territories_list,
                user_win,
                game_id  # Il WHERE usa game_id per identificare la riga da aggiornare
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
                    user_win
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
                round_id,
                user_owned_territories,
                user_owned_tanks,
                cpu_owned_territories,
                cpu_owned_tanks,
                user_owned_territories_list,
                cpu_owned_territories_list,
                user_win
            )


    
class InsertGameCommand:
    def __init__(self,conn,username,date):
        
        self.conn = conn
        self.username =username
        self.date = date
        
        self.insert_game_query = """
            INSERT INTO Games (username, date) 
            VALUES (%s, %s)
        """

        


    

# Servizio che gestisce i command
class CommandService:
    
    def handle_register_user(self, command: RegisterUserCommand): # command: RegisterUserCommand è un annotazione, suggerisce che command è di tipo RegisterUserCommand
        # Apertura della connessione al database
        with command.conn.cursor() as cursor:
            cursor.execute(command.register_user_command, (command.username, command.hashed_pwd,)) # Passiamo una tupla
            command.conn.commit()
            
    def handle_insert_data_command(self, command: InsertDataCommand):
        with command.conn.cursor() as cursor:
            cursor.execute(command.insert_command_query, command.values_tuple)
            command.conn.commit()
    
    def handle_insert_game_command(self, command: InsertGameCommand) -> int: # type int, serve semplicemente per migliorare la leggibilità del codice, suggerisce che la funzione dovrebbe restituire un intero
        with command.conn.cursor() as cursor:
            cursor.execute(command.insert_game_query, (command.username,command.date, command.state))
            command.conn.commit()
            game_id = cursor.lastrowid  # Restituisce l'ID dell'ultimo record inserito
            return game_id  # Restituiamo il game_id generato
            
            
    
        