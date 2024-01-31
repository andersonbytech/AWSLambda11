using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Udemy.Lambda_Annotations;

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
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "users/{userId}")]
    public async Task<User> FunctionHandler(string userId, ILambdaContext context)
    {
        return await _dynamodbcontext.LoadAsync<User>(userId.ToString());
    }


    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "users/")]
    public async Task PostFunctionHandler([FromBody] User user, ILambdaContext context)
    {
        await _dynamodbcontext.SaveAsync(user);
    }
}
public class User
{
    [DynamoDBHashKey]
    public string Id { get; set; }
    public string Nome { get; set; }

}