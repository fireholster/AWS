
using InfrastructureForMoon.Models;


namespace InfrastructureForMoon.Creds
{
    public class AwsCredentials : Amazon.Runtime.AWSCredentials
	{
		private readonly AppConfig _appConfig;

		public AwsCredentials(AppConfig appConfig)
		{
			_appConfig = appConfig;
		}

		public override Amazon.Runtime.ImmutableCredentials GetCredentials()
		{
			return new Amazon.Runtime.ImmutableCredentials(_appConfig.AwsAccessKey, _appConfig.AwsSecretKey, null);
		}	
	}
}
