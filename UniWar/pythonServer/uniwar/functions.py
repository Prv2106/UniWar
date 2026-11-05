"""Domain helpers for statistics aggregation and chart generation.

Provides the static continent-to-territory mapping, a lightweight wrapper
around the incoming gRPC request, battle metric aggregation, continent ownership
detection, and pie-chart rendering as base64-encoded PNG data.
"""

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
    """Normalized view of a ``DataRequest`` protobuf message."""

    def __init__(self, request):
        """Copies the request fields into plain Python structures.

        Args:
            request (statistics_pb2.DataRequest): Battle statistics payload.
        """
        self.is_user_turn = request.user_turn
        self.defending_territories = dict(request.defending_territories)
        self.attacking_territories = dict(request.attacking_territories)
        self.lost_territories = request.lost_territories


def process_data(data: Data):
    """Aggregates battle metrics according to the active player.

    Losses are attributed to the attacker or defender depending on whether the
    user or the CPU initiated the battle.

    Args:
        data (Data): Normalized battle statistics.

    Returns:
        tuple: User tanks lost, user territories lost, user perfect defenses,
        CPU tanks lost, CPU perfect defenses, CPU territories lost, user tanks
        lost while attacking, CPU tanks lost while attacking, user tanks lost
        while defending, and CPU tanks lost while defending.
    """
    if data.is_user_turn is True:  # User turn
        user_tanks_lost = user_tanks_lost_attacking = reduce(lambda x,y: x+y, data.attacking_territories.values(), 0) 
        cpu_tanks_lost = cpu_tanks_lost_defending= reduce(lambda x,y: x+y, data.defending_territories.values(), 0) 
        cpu_territories_lost = len(data.lost_territories)
        cpu_perfect_defenses = len(list(filter(lambda x: x == 0, data.defending_territories.values())))
        user_perfect_defenses = 0
        user_tanks_lost_defending = 0
        cpu_tanks_lost_attacking = 0
        user_territories_lost = 0        
            
    else:  # CPU turn
        user_tanks_lost = user_tanks_lost_defending = reduce(lambda x,y: x+y, data.defending_territories.values(), 0) 
        cpu_tanks_lost = cpu_tanks_lost_attacking = reduce(lambda x,y: x+y, data.attacking_territories.values(), 0) 
        user_territories_lost = len(data.lost_territories)
        user_perfect_defenses = len(list(filter(lambda x: x == 0, data.defending_territories.values())))
        cpu_perfect_defenses = 0
        cpu_tanks_lost_defending = 0
        user_tanks_lost_attacking = 0
        cpu_territories_lost = 0        
    
    return user_tanks_lost,user_territories_lost,user_perfect_defenses,cpu_tanks_lost,cpu_perfect_defenses,cpu_territories_lost,user_tanks_lost_attacking,cpu_tanks_lost_attacking,user_tanks_lost_defending, cpu_tanks_lost_defending

def get_owned_continents(owned_territories):
    """Returns the continents fully controlled by a player.

    Args:
        owned_territories (list[str]): Names of the territories owned.

    Returns:
        list[str]: Names of the continents whose territories are all owned.
    """
    owned_continents = []       
    for continent, territories in continents.items():
        # The generator expression is evaluated lazily; ``all`` returns True
        # only when every territory of the continent is owned.
        if all(territory in owned_territories for territory in territories):
            owned_continents.append(continent)

    return owned_continents


import matplotlib.pyplot as plt
import io
import base64

def generate_game_results_pie_chart(won, lost, incomplete):
    """Renders a pie chart of game outcomes as a base64-encoded PNG.

    Args:
        won (int): Number of games won.
        lost (int): Number of games lost.
        incomplete (int): Number of unfinished games.

    Returns:
        str: Base64-encoded PNG image data.
    """
    labels = ['Vinte', 'Perse', 'Incomplete']  # Category labels
    values = [won, lost, incomplete]  # Category values
    colors = ['#4CAF50', '#F44336', '#FFC107']  # Green, red, yellow
    explode = (0.1, 0.1, 0)  # Emphasize wins and losses

    # Drop categories whose value is zero
    filtered_labels = [label if size > 0 else "_" for label, size in zip(labels, values)]
    filtered_labels = [s for s in filtered_labels if s != "_"]
    filtered_values = [size for size in values if size > 0]
    filtered_explode = [e for e, size in zip(explode, values) if size > 0]
    filtered_colors = [c for c, size in zip(colors, values) if size > 0]

    plt.figure(figsize=(4,4))
    plt.pie(
        filtered_values,
        labels=filtered_labels,
        autopct=lambda p: f'{p:.1f}%' if p > 0 else '',
        colors=filtered_colors,
        explode=filtered_explode,
        startangle=140,
        textprops={'fontsize': 14, 'fontweight': 'bold'} 
    )

    # Encode the chart as a base64 PNG string
    img = io.BytesIO()
    plt.savefig(img, format='png', bbox_inches='tight')
    img.seek(0)
    graph_url = base64.b64encode(img.getvalue()).decode()
    plt.close()

    return graph_url
