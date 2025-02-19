from functools import reduce


continents = {
    "AmericaDelNord": ["Alaska", "Alberta", "StatiUnitiOccidentali", "StatiUnitiOrientali", "AmericaCentrale","Ontario", "TerritoriDelNordOvest", "Groenlandia", "Quebec"],
    "AmericaDelSud": ["Venezuela", "Perù", "Brasile", "Argentina"],
    "Africa": ["AfricaDelNord", "Egitto", "AfricaOrientale", "Congo", "AfricaDelSud", "Madagascar"],
    "Asia": ["Kamchatka", "Jacuzia", "Cita", "Giappone", "Cina", "Siam", "India", "MedioOriente", "Afghanistan", "Urali", "Siberia", "Mongolia"],
    "Europa": ["EuropaOccidentale", "EuropaMeridionale", "GranBretagna", "Islanda", "Ucraina", "EuropaSettentrionale", "Scandinavia"],
    "Oceania": ["AustraliaOccidentale", "NuovaGuinea", "Indonesia", "AustraliaOrientale"]
}



class Data:

    def __init__(self,request):
        self.is_user_turn = request.user_turn
        self.defending_territories =dict(request.defending_territories) 
        self.attacking_territories = dict(request.attacking_territories)
        self.lost_territories = request.lost_territories


def process_data(data: Data):

    if data.is_user_turn is True: # Turno utente
        user_tanks_lost = user_tanks_lost_attacking = reduce(lambda x,y: x+y, data.attacking_territories.values()) 
        cpu_tanks_lost = cpu_tanks_lost_defending= reduce(lambda x,y: x+y, data.defending_territories.values()) 
        cpu_territories_lost = len(data.lost_territories)
        cpu_perfect_defenses = len(list(filter(lambda x: x == 0, data.defending_territories.values())))
        user_perfect_defenses = 0
        user_tanks_lost_defending = 0
        cpu_tanks_lost_attacking = 0
        user_territories_lost = 0
        
        
            
    else: # Turno della CPU
        user_tanks_lost = user_tanks_lost_defending = reduce(lambda x,y: x+y, data.defending_territories.values()) 
        cpu_tanks_lost = cpu_tanks_lost_attacking = reduce(lambda x,y: x+y, data.attacking_territories.values()) 
        user_territories_lost = len(data.lost_territories)
        user_perfect_defenses = len(list(filter(lambda x: x == 0, data.defending_territories.values())))
        cpu_perfect_defenses = 0
        cpu_tanks_lost_defending = 0
        user_tanks_lost_attacking = 0
        cpu_territories_lost = 0
        
    
    return user_tanks_lost,user_territories_lost,user_perfect_defenses,cpu_tanks_lost,cpu_perfect_defenses,cpu_territories_lost,user_tanks_lost_attacking,cpu_tanks_lost_attacking,user_tanks_lost_defending, cpu_tanks_lost_defending



# Funzione che restituisce una lista di stringhe contenente il nome dei contenenti posseduti data una lista di territori posseduti
def get_owned_continents(owned_territories):
    owned_continents = []       
    for continent, territories in continents.items():
        # Usiamo la sintassi territory in owned_territories for territory in territories
        # in questo modo otteniamo un effetto simile ad una list comprehension in cui abbiamo dei valori booleani
        # la differenza sta nel fatto che anziché creare una list creiamo invece un iteratore, facciamo una lazy evaluation
        # all restituisce True solo se ogni elemento dell'iteratore è True
        if all(territory in owned_territories for territory in territories):
            owned_continents.append(continent)

    return owned_continents



