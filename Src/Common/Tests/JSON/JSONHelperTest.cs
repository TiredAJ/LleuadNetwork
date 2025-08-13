using System.Numerics;

using Common.Json;
using Common.Tests.JSON.Files;

using NUnit.Framework;

using UserResult = CSharpFunctionalExtensions.Result<Common.Tests.JSON.Files.ExampleUser>;

namespace Common.Tests.JSON;

[TestFixture]
public class JSONHelperTest
{
    private const string TEST_FILE_PATH = @"Tests/JSON/Files/";
    private string SerialisingFilePath => Path.Combine(TEST_FILE_PATH, "SerialisedFile.json");
    private string DeserialisingFilePath => Path.Combine(TEST_FILE_PATH, "Example.json");
    private ExampleUser RoxUser;
    
    [SetUp]
    public void Setup() {
        if (File.Exists(SerialisingFilePath))
        { File.Delete(SerialisingFilePath); }
        
        RoxUser = new ExampleUser {
            DOB = new DateOnly(2001, 5, 15),
            Coordinates = new Vector2(51.4822415f, -3.1569247f),
            IsAdmin = true,
            Name = "Roxanne T",
            Perms = [
                new SecurityPermission() { Name = "Open Access", Passphrase = "LNGDYKJ", SecurityKey = [45, 65, 32, 78] }
            ]
        };
    }
    
    [Test]
    public void Serialise() {
        Stream JStream = FileWriter(Path.Combine(TEST_FILE_PATH, "SerialisedFile.json"));
        
        Assert.DoesNotThrow(() => JSONHelper.Serialise(JStream, RoxUser, UserJSONContext.Default));
    }
    
    [Test]
    public void SerialiseTypeWithoutContext() {
        Stream JStream = FileWriter(Path.Combine(TEST_FILE_PATH, "SerialisedFile.json"));
        
        Assert.Throws<InvalidOperationException>(() => JSONHelper.Serialise(JStream, RoxUser));
    }
    
    [Test]
    public void Deserialise() {
        Stream JStream = FileReader(DeserialisingFilePath);

        UserResult UserRes = default;
        
        Assert.DoesNotThrow(() => {
            UserRes = JSONHelper.Deserialise<ExampleUser>(JStream, UserJSONContext.Default);
        });
        
        Assert.That(UserRes.IsSuccess, Is.True);

#pragma warning disable CFE0001
        ExampleUser ElisaUser = UserRes!.Value;
#pragma warning restore CFE0001
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ElisaUser.Name, Is.EqualTo("Elisa"));
            Assert.That(ElisaUser.DOB, Is.EqualTo(new DateOnly(2002, 8, 6)));
            Assert.That(ElisaUser.Coordinates, Is.EqualTo(RoxUser.Coordinates));
            Assert.That(ElisaUser.IsAdmin, Is.True);
        }
        
        Assert.That(ElisaUser.Perms, Has.Count.EqualTo(2));

        SecurityPermission Perm1 = ElisaUser.Perms.ToList()[0];
        SecurityPermission Perm2 = ElisaUser.Perms.ToList()[0];
        
        using (Assert.EnterMultipleScope())
        {
            ValidatePerms(Perm1, "Open Access", "LNGDYKJ", "fruit"u8.ToArray());
            ValidatePerms(Perm2, "Controller Access", "LNGDYKJ", "fruit"u8.ToArray());
        }
    }

    [Test]
    public void DeserialiseTypeWithoutContext() {
        Stream JStream = FileReader(DeserialisingFilePath);
        
        Assert.Throws<InvalidOperationException>(
            () => JSONHelper.Deserialise<ExampleUser>(JStream));
    }
    
    private void ValidatePerms(SecurityPermission _Perm, string _Name, string _Passphrase, byte[] _SecurityKey) {
        Assert.That(_Perm.Name, Is.EqualTo("Open Access"));
        Assert.That(_Perm.Passphrase, Is.EqualTo(_Passphrase));
    }

    private Stream FileReader(string _Path)
        => new StreamReader(_Path).BaseStream;

    private Stream FileWriter(string _Path)
        => new StreamWriter(_Path).BaseStream;
}
