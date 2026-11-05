"""MySQL connection settings used by the gRPC server.

The host name matches the ``mysql_container`` service declared in
``docker-compose.yml``.
"""

db_config = {
    "host": "mysql_container",
    "user": "alberto_giuseppe",
    "password": "progetto",
    "database": "uniwar"
}