using LogicPOS.Core;

namespace LogicPOS.Core.Tests;

[Collection(UiCultureCollection.Name)]
public sealed class UiCaptionTests
{
    [Fact]
    public void Display_translates_known_captions_in_english()
    {
        var previous = Capture();
        try
        {
            UiCulture.SetCachedForTests("en-US");
            Assert.Equal("Save", UiCaption.Display("Guardar"));
            Assert.Equal("Customer", UiCaption.Display("Cliente"));
            Assert.Equal("2 of 5 required steps completed", UiCaption.Display("2 de 5 passos obrigatórios concluídos"));
            Assert.Equal("Recommended next step: Save", UiCaption.Display("Próximo passo recomendado: Guardar"));
            Assert.Equal("Unit price", UiCaption.Display("Unit price"));
        }
        finally
        {
            Restore(previous);
        }
    }

    [Fact]
    public void Display_uses_the_same_english_for_en_GB()
    {
        var previous = Capture();
        try
        {
            UiCulture.SetCachedForTests("en-GB");
            Assert.Equal("Save", UiCaption.Display("Guardar"));
        }
        finally
        {
            Restore(previous);
        }
    }

    [Fact]
    public void Display_keeps_portuguese_outside_english_and_restores_it()
    {
        var previous = Capture();
        try
        {
            UiCulture.SetCachedForTests("pt-PT");
            Assert.Equal("Guardar", UiCaption.Display("Guardar"));
            Assert.Equal("Save", UiCaption.Display("Save"));
            Assert.Equal("Guardar", UiCaption.Revert("Save"));
            Assert.Equal("Moçambique", UiCaption.Display("Moçambique"));

            UiCulture.SetCachedForTests("fr-FR");
            Assert.Equal("Guardar", UiCaption.Display("Guardar"));
            Assert.Equal("Documents", UiCaption.Display("Documents"));
            Assert.Equal("Documentos", UiCaption.Revert("Documents"));
        }
        finally
        {
            Restore(previous);
        }
    }

    private static string Capture() => UiCulture.Read(null);

    private static void Restore(string previous)
    {
        UiCulture.SetCachedForTests(previous);
    }
}
