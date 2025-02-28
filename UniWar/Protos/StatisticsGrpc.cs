// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/statistics.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Statistics {
  public static partial class StatisticsService
  {
    static readonly string __ServiceName = "statistics.StatisticsService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.DataRequest> __Marshaller_statistics_DataRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.DataRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.Response> __Marshaller_statistics_Response = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.Response.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.SignInCredentials> __Marshaller_statistics_SignInCredentials = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.SignInCredentials.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.SignUpCredentials> __Marshaller_statistics_SignUpCredentials = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.SignUpCredentials.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.Username> __Marshaller_statistics_Username = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.Username.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.StatisticRequest> __Marshaller_statistics_StatisticRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.StatisticRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.StatisticsResponse> __Marshaller_statistics_StatisticsResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.StatisticsResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.GameInfoList> __Marshaller_statistics_GameInfoList = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.GameInfoList.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.NewGameResponse> __Marshaller_statistics_NewGameResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.NewGameResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Statistics.EndGameRequest> __Marshaller_statistics_EndGameRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Statistics.EndGameRequest.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.DataRequest, global::Statistics.Response> __Method_send_data = new grpc::Method<global::Statistics.DataRequest, global::Statistics.Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "send_data",
        __Marshaller_statistics_DataRequest,
        __Marshaller_statistics_Response);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.SignInCredentials, global::Statistics.Response> __Method_sign_in = new grpc::Method<global::Statistics.SignInCredentials, global::Statistics.Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "sign_in",
        __Marshaller_statistics_SignInCredentials,
        __Marshaller_statistics_Response);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.SignUpCredentials, global::Statistics.Response> __Method_sign_up = new grpc::Method<global::Statistics.SignUpCredentials, global::Statistics.Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "sign_up",
        __Marshaller_statistics_SignUpCredentials,
        __Marshaller_statistics_Response);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.Username, global::Statistics.Response> __Method_username_check = new grpc::Method<global::Statistics.Username, global::Statistics.Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "username_check",
        __Marshaller_statistics_Username,
        __Marshaller_statistics_Response);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.StatisticRequest, global::Statistics.StatisticsResponse> __Method_get_statistics = new grpc::Method<global::Statistics.StatisticRequest, global::Statistics.StatisticsResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "get_statistics",
        __Marshaller_statistics_StatisticRequest,
        __Marshaller_statistics_StatisticsResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.Username, global::Statistics.GameInfoList> __Method_get_games = new grpc::Method<global::Statistics.Username, global::Statistics.GameInfoList>(
        grpc::MethodType.Unary,
        __ServiceName,
        "get_games",
        __Marshaller_statistics_Username,
        __Marshaller_statistics_GameInfoList);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.Username, global::Statistics.NewGameResponse> __Method_new_game = new grpc::Method<global::Statistics.Username, global::Statistics.NewGameResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "new_game",
        __Marshaller_statistics_Username,
        __Marshaller_statistics_NewGameResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Statistics.EndGameRequest, global::Statistics.Response> __Method_end_game = new grpc::Method<global::Statistics.EndGameRequest, global::Statistics.Response>(
        grpc::MethodType.Unary,
        __ServiceName,
        "end_game",
        __Marshaller_statistics_EndGameRequest,
        __Marshaller_statistics_Response);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Statistics.StatisticsReflection.Descriptor.Services[0]; }
    }

    /// <summary>Client for StatisticsService</summary>
    public partial class StatisticsServiceClient : grpc::ClientBase<StatisticsServiceClient>
    {
      /// <summary>Creates a new client for StatisticsService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public StatisticsServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for StatisticsService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public StatisticsServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected StatisticsServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected StatisticsServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// da richiamare ogni volta che si conclude una fase di attacco da parte di un giocatore
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response send_data(global::Statistics.DataRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return send_data(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// da richiamare ogni volta che si conclude una fase di attacco da parte di un giocatore
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response send_data(global::Statistics.DataRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_send_data, null, options, request);
      }
      /// <summary>
      /// da richiamare ogni volta che si conclude una fase di attacco da parte di un giocatore
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> send_dataAsync(global::Statistics.DataRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return send_dataAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// da richiamare ogni volta che si conclude una fase di attacco da parte di un giocatore
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> send_dataAsync(global::Statistics.DataRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_send_data, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response sign_in(global::Statistics.SignInCredentials request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return sign_in(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response sign_in(global::Statistics.SignInCredentials request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_sign_in, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> sign_inAsync(global::Statistics.SignInCredentials request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return sign_inAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> sign_inAsync(global::Statistics.SignInCredentials request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_sign_in, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response sign_up(global::Statistics.SignUpCredentials request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return sign_up(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response sign_up(global::Statistics.SignUpCredentials request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_sign_up, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> sign_upAsync(global::Statistics.SignUpCredentials request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return sign_upAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> sign_upAsync(global::Statistics.SignUpCredentials request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_sign_up, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response username_check(global::Statistics.Username request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return username_check(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response username_check(global::Statistics.Username request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_username_check, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> username_checkAsync(global::Statistics.Username request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return username_checkAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> username_checkAsync(global::Statistics.Username request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_username_check, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.StatisticsResponse get_statistics(global::Statistics.StatisticRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return get_statistics(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.StatisticsResponse get_statistics(global::Statistics.StatisticRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_get_statistics, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.StatisticsResponse> get_statisticsAsync(global::Statistics.StatisticRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return get_statisticsAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.StatisticsResponse> get_statisticsAsync(global::Statistics.StatisticRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_get_statistics, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.GameInfoList get_games(global::Statistics.Username request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return get_games(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.GameInfoList get_games(global::Statistics.Username request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_get_games, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.GameInfoList> get_gamesAsync(global::Statistics.Username request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return get_gamesAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.GameInfoList> get_gamesAsync(global::Statistics.Username request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_get_games, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.NewGameResponse new_game(global::Statistics.Username request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return new_game(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.NewGameResponse new_game(global::Statistics.Username request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_new_game, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.NewGameResponse> new_gameAsync(global::Statistics.Username request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return new_gameAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.NewGameResponse> new_gameAsync(global::Statistics.Username request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_new_game, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response end_game(global::Statistics.EndGameRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return end_game(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Statistics.Response end_game(global::Statistics.EndGameRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_end_game, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> end_gameAsync(global::Statistics.EndGameRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return end_gameAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Statistics.Response> end_gameAsync(global::Statistics.EndGameRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_end_game, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override StatisticsServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new StatisticsServiceClient(configuration);
      }
    }

  }
}
#endregion
