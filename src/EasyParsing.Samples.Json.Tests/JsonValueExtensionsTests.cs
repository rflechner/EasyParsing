using EasyParsing.Samples.Json;
using FluentAssertions;
using NUnit.Framework;

namespace EasyParsing.Samples.Tests;

public class JsonValueExtensionsTests
{
    [Test]
    public void SelectSimpleProperty_Should_Success()
    {
        var text = File.ReadAllText("payload1.json");
        var json = JsonParser.ParseJson(text);

        var age = json.Select("age")?.ReadAsInt();
        var name = json.Select("name")?.ReadAsString();
        var strangProperty = json.Select("l'age du \"capitaine\" Toto")?.ReadAsDecimal();
        var message = json.Select("message")?.ReadAsString();

        age!.Should().Be(39);
        name!.Should().Be("Toto lol");
        strangProperty!.Should().Be(37.5m);
        message!.Should().Be("Hello, my name is \"The Giant\". I'm fine.");
    }
}