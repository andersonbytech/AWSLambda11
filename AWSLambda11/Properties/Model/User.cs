using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda11.Properties.Model
{
    public class User
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string Nome { get; set; }    

    }
}
