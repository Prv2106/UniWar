# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# NO CHECKED-IN PROTOBUF GENCODE
# source: statistics.proto
# Protobuf Python Version: 5.28.1
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import runtime_version as _runtime_version
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
_runtime_version.ValidateProtobufRuntimeVersion(
    _runtime_version.Domain.PUBLIC,
    5,
    28,
    1,
    '',
    'statistics.proto'
)
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x10statistics.proto\x12\nstatistics\"1\n\x0e\x45ndGameRequest\x12\x0f\n\x07game_id\x18\x01 \x01(\x05\x12\x0e\n\x06is_win\x18\x02 \x01(\x08\"#\n\x10StatisticRequest\x12\x0f\n\x07game_id\x18\x01 \x01(\x05\"\x1c\n\x08Username\x12\x10\n\x08username\x18\x01 \x01(\t\"\xff\x03\n\x0b\x44\x61taRequest\x12\x10\n\x08username\x18\x01 \x01(\t\x12\x10\n\x08round_id\x18\x02 \x01(\x05\x12\x11\n\tuser_turn\x18\x03 \x01(\x08\x12\x18\n\x10lost_territories\x18\x04 \x03(\t\x12P\n\x15\x64\x65\x66\x65nding_territories\x18\x05 \x03(\x0b\x32\x31.statistics.DataRequest.DefendingTerritoriesEntry\x12P\n\x15\x61ttacking_territories\x18\x06 \x03(\x0b\x32\x31.statistics.DataRequest.AttackingTerritoriesEntry\x12\x18\n\x10user_owned_tanks\x18\x07 \x01(\x05\x12\x1e\n\x16user_owned_territories\x18\x08 \x03(\t\x12\x0f\n\x07game_id\x18\t \x01(\x05\x12\x1d\n\x15\x63pu_owned_territories\x18\n \x03(\t\x12\x17\n\x0f\x63pu_owned_tanks\x18\x0b \x01(\x05\x1a;\n\x19\x44\x65\x66\x65ndingTerritoriesEntry\x12\x0b\n\x03key\x18\x01 \x01(\t\x12\r\n\x05value\x18\x02 \x01(\x05:\x02\x38\x01\x1a;\n\x19\x41ttackingTerritoriesEntry\x12\x0b\n\x03key\x18\x01 \x01(\t\x12\r\n\x05value\x18\x02 \x01(\x05:\x02\x38\x01\"7\n\x11SignInCredentials\x12\x10\n\x08username\x18\x01 \x01(\t\x12\x10\n\x08password\x18\x02 \x01(\t\"7\n\x11SignUpCredentials\x12\x10\n\x08username\x18\x01 \x01(\t\x12\x10\n\x08password\x18\x02 \x01(\t\"+\n\x08Response\x12\x0f\n\x07message\x18\x01 \x01(\t\x12\x0e\n\x06status\x18\x02 \x01(\x08\"3\n\x08GameInfo\x12\n\n\x02id\x18\x01 \x01(\x05\x12\x0c\n\x04\x64\x61te\x18\x02 \x01(\t\x12\r\n\x05state\x18\x03 \x01(\x05\"t\n\x0cGameInfoList\x12\x0f\n\x07message\x18\x01 \x01(\t\x12\x0e\n\x06status\x18\x02 \x01(\x08\x12#\n\x05games\x18\x03 \x03(\x0b\x32\x14.statistics.GameInfo\x12\x1e\n\x16game_results_pie_chart\x18\x04 \x01(\t\"\xda\x05\n\x12StatisticsResponse\x12\x1e\n\x16user_owned_territories\x18\x01 \x01(\x05\x12\x18\n\x10user_owned_tanks\x18\x02 \x01(\x05\x12\x1d\n\x15\x63pu_owned_territories\x18\x03 \x01(\x05\x12\x17\n\x0f\x63pu_owned_tanks\x18\x04 \x01(\x05\x12\x1d\n\x15user_owned_continents\x18\x05 \x03(\t\x12\x1c\n\x14\x63pu_owned_continents\x18\x06 \x03(\t\x12\x10\n\x08user_win\x18\x07 \x01(\x05\x12!\n\x19user_tanks_lost_per_round\x18\x08 \x01(\x02\x12 \n\x18\x63pu_tanks_lost_per_round\x18\t \x01(\x02\x12+\n#user_tanks_lost_attacking_per_round\x18\n \x01(\x02\x12*\n\"cpu_tanks_lost_attacking_per_round\x18\x0b \x01(\x02\x12+\n#user_tanks_lost_defending_per_round\x18\x0c \x01(\x02\x12*\n\"cpu_tanks_lost_defending_per_round\x18\r \x01(\x02\x12\x1d\n\x15user_perfect_defenses\x18\x0e \x01(\x05\x12\x1c\n\x14\x63pu_perfect_defenses\x18\x0f \x01(\x05\x12\'\n\x1fuser_territories_lost_per_round\x18\x10 \x01(\x02\x12&\n\x1e\x63pu_territories_lost_per_round\x18\x11 \x01(\x02\x12%\n\x1duser_map_ownership_percentage\x18\x12 \x01(\x02\x12$\n\x1c\x63pu_map_ownership_percentage\x18\x13 \x01(\x02\x12\x0f\n\x07message\x18\x14 \x01(\t\x12\x0e\n\x06status\x18\x15 \x01(\x08\x12\x10\n\x08round_id\x18\x16 \x01(\x05\"C\n\x0fNewGameResponse\x12\x0f\n\x07game_id\x18\x01 \x01(\x05\x12\x0f\n\x07message\x18\x02 \x01(\t\x12\x0e\n\x06status\x18\x03 \x01(\x08\x32\x97\x04\n\x11StatisticsService\x12:\n\tsend_data\x12\x17.statistics.DataRequest\x1a\x14.statistics.Response\x12>\n\x07sign_in\x12\x1d.statistics.SignInCredentials\x1a\x14.statistics.Response\x12>\n\x07sign_up\x12\x1d.statistics.SignUpCredentials\x1a\x14.statistics.Response\x12<\n\x0eusername_check\x12\x14.statistics.Username\x1a\x14.statistics.Response\x12N\n\x0eget_statistics\x12\x1c.statistics.StatisticRequest\x1a\x1e.statistics.StatisticsResponse\x12;\n\tget_games\x12\x14.statistics.Username\x1a\x18.statistics.GameInfoList\x12=\n\x08new_game\x12\x14.statistics.Username\x1a\x1b.statistics.NewGameResponse\x12<\n\x08\x65nd_game\x12\x1a.statistics.EndGameRequest\x1a\x14.statistics.Responseb\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'statistics_pb2', _globals)
if not _descriptor._USE_C_DESCRIPTORS:
  DESCRIPTOR._loaded_options = None
  _globals['_DATAREQUEST_DEFENDINGTERRITORIESENTRY']._loaded_options = None
  _globals['_DATAREQUEST_DEFENDINGTERRITORIESENTRY']._serialized_options = b'8\001'
  _globals['_DATAREQUEST_ATTACKINGTERRITORIESENTRY']._loaded_options = None
  _globals['_DATAREQUEST_ATTACKINGTERRITORIESENTRY']._serialized_options = b'8\001'
  _globals['_ENDGAMEREQUEST']._serialized_start=32
  _globals['_ENDGAMEREQUEST']._serialized_end=81
  _globals['_STATISTICREQUEST']._serialized_start=83
  _globals['_STATISTICREQUEST']._serialized_end=118
  _globals['_USERNAME']._serialized_start=120
  _globals['_USERNAME']._serialized_end=148
  _globals['_DATAREQUEST']._serialized_start=151
  _globals['_DATAREQUEST']._serialized_end=662
  _globals['_DATAREQUEST_DEFENDINGTERRITORIESENTRY']._serialized_start=542
  _globals['_DATAREQUEST_DEFENDINGTERRITORIESENTRY']._serialized_end=601
  _globals['_DATAREQUEST_ATTACKINGTERRITORIESENTRY']._serialized_start=603
  _globals['_DATAREQUEST_ATTACKINGTERRITORIESENTRY']._serialized_end=662
  _globals['_SIGNINCREDENTIALS']._serialized_start=664
  _globals['_SIGNINCREDENTIALS']._serialized_end=719
  _globals['_SIGNUPCREDENTIALS']._serialized_start=721
  _globals['_SIGNUPCREDENTIALS']._serialized_end=776
  _globals['_RESPONSE']._serialized_start=778
  _globals['_RESPONSE']._serialized_end=821
  _globals['_GAMEINFO']._serialized_start=823
  _globals['_GAMEINFO']._serialized_end=874
  _globals['_GAMEINFOLIST']._serialized_start=876
  _globals['_GAMEINFOLIST']._serialized_end=992
  _globals['_STATISTICSRESPONSE']._serialized_start=995
  _globals['_STATISTICSRESPONSE']._serialized_end=1725
  _globals['_NEWGAMERESPONSE']._serialized_start=1727
  _globals['_NEWGAMERESPONSE']._serialized_end=1794
  _globals['_STATISTICSSERVICE']._serialized_start=1797
  _globals['_STATISTICSSERVICE']._serialized_end=2332
# @@protoc_insertion_point(module_scope)
