import bcrypt
import re



# Command per la registrazione dell'utente
class RegisterUserCommand:
    def __init__(self, username, password, conn):
        
        # verifica che la password non sia vuota
        if not password:
            print("Password non inserita", flush = True)
            raise ValueError("password non inserita")
        
        if not RegisterUserCommand.validate_username(username):
           print("Username non valido", flush = True)
           raise ValueError("Invalid Username")
         
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
        username_regex = r"^[A-Z][a-zA-Z0-9_-]{1,}$" # Username che inizia con una lettera maiuscola e che deve avere almeno 1 altro carattere tra quelli indicati nella classe []
        return re.match(username_regex, username) is not None
    
    
    
    
   
    

# Servizio che gestisce i command
class CommandService:
    
    def handle_register_user(self, command: RegisterUserCommand): # command: RegisterUserCommand è un annotazione, suggerisce che command è di tipo RegisterUserCommand
        # Apertura della connessione al database
        with command.conn.cursor() as cursor:
            cursor.execute(command.register_user_command, (command.username, command.hashed_pwd,)) # Passiamo una tupla
            command.conn.commit()