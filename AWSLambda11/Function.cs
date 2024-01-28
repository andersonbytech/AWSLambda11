using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Transform;
using AWSLambda11.Model;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda11;

public class Function
{
    private readonly DynamoDBContext _dynamodbcontext;

    public Function()
    {
        _dynamodbcontext = new DynamoDBContext(new AmazonDynamoDBClient());
    }


    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        return request.RequestContext.Http.Method.ToUpper() switch
        {
            "GET" => await HandleGetRequest(request),
            "POST" => await HandlePostRequest(request),
            "DELETE" => await HandleDeleteRequest(request)
        };
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> HandleDeleteRequest(APIGatewayHttpApiV2ProxyRequest request)
    {
        request.PathParameters.TryGetValue("userId", out var userId);
        var user = await _dynamodbcontext.LoadAsync<User>(userId);
        if (user != null)
        {
            await _dynamodbcontext.DeleteAsync(user);
            return new APIGatewayHttpApiV2ProxyResponse()
            {
                StatusCode = 204
            };
        }
        return BadRespose("Registro não encontrado");
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> HandleGetRequest(APIGatewayHttpApiV2ProxyRequest request)
    {
        request.PathParameters.TryGetValue("userId", out var userId);
        if (userId != null)
        {

            var user = await _dynamodbcontext.LoadAsync<User>(userId);

            if (user != null)
            {
                return new APIGatewayHttpApiV2ProxyResponse()
                {

                    Body = JsonSerializer.Serialize(user),
                    StatusCode = 200
                };
            }
        }
        return BadRespose("UserId inválido");
    }
    private async Task<APIGatewayHttpApiV2ProxyResponse> HandlePostRequest(
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var user = JsonSerializer.Deserialize<User>(request.Body);

        if (user == null)
        {
            return BadRespose("Detalhes do UserId inválidos");

        }

        await _dynamodbcontext.SaveAsync(user);
        return new APIGatewayHttpApiV2ProxyResponse()
        {
            StatusCode = 201
        };

    }
    private static APIGatewayHttpApiV2ProxyResponse BadRespose(string message)
    {
        return new APIGatewayHttpApiV2ProxyResponse()
        {
            Body = message,
            StatusCode = 404
        };
    }
}
