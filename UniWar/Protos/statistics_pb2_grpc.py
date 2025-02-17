# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc
import warnings

import statistics_pb2 as statistics__pb2

GRPC_GENERATED_VERSION = '1.68.0'
GRPC_VERSION = grpc.__version__
_version_not_supported = False

try:
    from grpc._utilities import first_version_is_lower
    _version_not_supported = first_version_is_lower(GRPC_VERSION, GRPC_GENERATED_VERSION)
except ImportError:
    _version_not_supported = True

if _version_not_supported:
    raise RuntimeError(
        f'The grpc package installed is at version {GRPC_VERSION},'
        + f' but the generated code in statistics_pb2_grpc.py depends on'
        + f' grpcio>={GRPC_GENERATED_VERSION}.'
        + f' Please upgrade your grpc module to grpcio>={GRPC_GENERATED_VERSION}'
        + f' or downgrade your generated code using grpcio-tools<={GRPC_VERSION}.'
    )


class StatisticsServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.SendStatistics = channel.unary_unary(
                '/statistics.StatisticsService/SendStatistics',
                request_serializer=statistics__pb2.StatisticsCollection.SerializeToString,
                response_deserializer=statistics__pb2.Response.FromString,
                _registered_method=True)
        self.SignIn = channel.unary_unary(
                '/statistics.StatisticsService/SignIn',
                request_serializer=statistics__pb2.SignInCredentials.SerializeToString,
                response_deserializer=statistics__pb2.Response.FromString,
                _registered_method=True)
        self.SignUp = channel.unary_unary(
                '/statistics.StatisticsService/SignUp',
                request_serializer=statistics__pb2.SignUpCredentials.SerializeToString,
                response_deserializer=statistics__pb2.Response.FromString,
                _registered_method=True)


class StatisticsServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def SendStatistics(self, request, context):
        """da richiamare ogni volta che si conclude una fase di attacco da parte di un giocatore
        """
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def SignIn(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def SignUp(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_StatisticsServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'SendStatistics': grpc.unary_unary_rpc_method_handler(
                    servicer.SendStatistics,
                    request_deserializer=statistics__pb2.StatisticsCollection.FromString,
                    response_serializer=statistics__pb2.Response.SerializeToString,
            ),
            'SignIn': grpc.unary_unary_rpc_method_handler(
                    servicer.SignIn,
                    request_deserializer=statistics__pb2.SignInCredentials.FromString,
                    response_serializer=statistics__pb2.Response.SerializeToString,
            ),
            'SignUp': grpc.unary_unary_rpc_method_handler(
                    servicer.SignUp,
                    request_deserializer=statistics__pb2.SignUpCredentials.FromString,
                    response_serializer=statistics__pb2.Response.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'statistics.StatisticsService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('statistics.StatisticsService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class StatisticsService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def SendStatistics(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/statistics.StatisticsService/SendStatistics',
            statistics__pb2.StatisticsCollection.SerializeToString,
            statistics__pb2.Response.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def SignIn(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/statistics.StatisticsService/SignIn',
            statistics__pb2.SignInCredentials.SerializeToString,
            statistics__pb2.Response.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def SignUp(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/statistics.StatisticsService/SignUp',
            statistics__pb2.SignUpCredentials.SerializeToString,
            statistics__pb2.Response.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)
