using Common.Json;
using Common.Tests.JSON.Files;

using CSharpFunctionalExtensions;

using NUnit.Framework;

namespace Common.Tests.JSON;

[TestFixture]
public class JSONValidatorTest
{
    private const string TEST_FILE_PATH = @"Tests/JSON/Files/";
    private string InvalidFile => Path.Combine(TEST_FILE_PATH, "Invalid.json");
    private string InvalidVersionFile => Path.Combine(TEST_FILE_PATH, "InvalidVersion.json");
    private string ValidFile => Path.Combine(TEST_FILE_PATH, "Example.json");
    
    [Test]
    public void Validate() {
        Maybe<Exception> R;
        
        Assert.DoesNotThrow(() => {
            R = JsonValidator.ValidateJson<ExampleUser>(ValidFile, out Stream? JStream);
        });
    }
    
    [Test]
    public void ValidateInvalid() {

        string FailureMsg =
            $"JSON schema validation failed for [{InvalidFile}] with the following errors: [" +
            $"NoAdditionalPropertiesAllowed: #/FakeField]";

        Maybe<Exception> R = default;
        Stream? JStream = null;
        
        Assert.DoesNotThrow(() => {
             R = JsonValidator.ValidateJson<ExampleUser>(InvalidFile, out JStream);
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(R.Value.Message, Is.EqualTo(FailureMsg));
            Assert.That(JStream, Is.Null);
        }
    }
    
    [Test]
    public void ValidateInvalidVersion() {

        string FailureMsg =
            $"ExampleUser schema failed version validation. Expected version: [1], File version: [3].";

        Maybe<Exception> R = default;
        Stream? JStream = null;
        
        Assert.DoesNotThrow(() => {
             R = JsonValidator.ValidateJson<ExampleUser>(InvalidVersionFile, out JStream);
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(R.Value.Message, Is.EqualTo(FailureMsg));
            Assert.That(JStream, Is.Null);
        }
    }
}
