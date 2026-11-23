using System.Collections.Generic;

namespace LogicPOS.Persistence.DataFixes;

/// <summary>
/// Conditional SQL used by the FixSeedTexts2035 EF migrations (SQLite / MySQL / SQL Server).
/// Each UPDATE matches the exact previous seed value so customer edits are left untouched.
/// </summary>
public static class FixSeedTexts2035Statements
{
    public static readonly (string Old, string New)[] ArticleDesignations =
    [
        ("Duas Quintas", "Duas quintas"),
        ("Chá Vermelho", "Chá vermelho"),
        ("Mil Folhas", "Mil folhas"),
        ("Santàl Nectar", "Santàl nectar"),
        ("Chá Branco", "Chá branco"),
        ("Sopa Da Pedra", "Sopa da pedra"),
        ("Herdade Do Sobroso", "Herdade do sobroso"),
        ("Quinta de Cabriz Branco", "Quinta de cabriz branco"),
        ("Tarte de Maçã", "Tarte de maçã"),
        ("Alvarinho Couto de Frades", "Alvarinho couto de frades"),
        ("Gelado Baunilha", "Gelado baunilha"),
        ("Sopa de Peixe", "Sopa de peixe"),
        ("Croissants de Galinha", "Croissants de galinha"),
        ("Chá Tisana", "Chá tisana"),
        ("Sande de Queijo", "Sande de queijo"),
        ("Bifana no Pão", "Bifana no pão"),
        ("Carioca de Café", "Carioca de café"),
        ("Super Bock Abadia", "Super bock abadia"),
        ("Sta Valha Reserva", "Sta valha reserva"),
        ("Sagres Mini", "Sagres mini"),
        ("Meia de Leite", "Meia de leite"),
        ("Croissant Misto", "Croissant misto"),
        ("Super Bock Mini Stout", "Super bock mini stout"),
        ("Gelado Natas", "Gelado natas"),
        ("Pizza Especial", "Pizza especial"),
        ("Tosta de Fiambre", "Tosta de fiambre"),
        ("Queijo Ovelha Curado", "Queijo ovelha curado"),
        ("Baba de Camelo", "Baba de camelo"),
        ("Sande Americana", "Sande americana"),
        ("Super Bock 33cl", "Super bock 33cl"),
        ("Lapa Dos Gaviões", "Lapa dos gaviões"),
        ("Galão Normal", "Galão normal"),
        ("Gelado Morango", "Gelado morango"),
        ("Sanduíche de Frango", "Sanduíche de frango"),
        ("Bola de Berlim", "Bola de berlim"),
        ("Vila Régia", "Vila régia"),
        ("Salada de Fruta", "Salada de fruta"),
        ("Pata de Veado", "Pata de veado"),
        ("Quinta de Cabriz", "Quinta de cabriz"),
        ("Água Luso 33cl", "Água luso 33cl"),
        ("Super Bock S.A", "Super bock s.a"),
        ("Sagres Bohemia", "Sagres bohemia"),
        ("Água Castelo", "Água castelo"),
        ("Duas Quintas Branco", "Duas quintas branco"),
        ("Gelado Limão", "Gelado limão"),
        ("Água Luso 1l", "Água luso 1l"),
        ("Queijo Serra", "Queijo serra"),
        ("Tarte de Chila", "Tarte de chila"),
        ("Luso Sabores", "Luso sabores"),
        ("Pizza Light", "Pizza light"),
        ("Cuca Mini", "Cuca mini"),
        ("Monte Velho Branco", "Monte velho branco"),
        ("Super Bock Mini", "Super bock mini"),
        ("Melão com Presunto", "Melão com presunto"),
        ("Chá Preto", "Chá preto"),
        ("Croissant de Chocolate", "Croissant de chocolate"),
        ("Monsaraz Branco", "Monsaraz branco"),
        ("Piornos Reserva", "Piornos reserva"),
        ("Vila Régia B", "Vila régia b"),
        ("Sagres S.A", "Sagres s.a"),
        ("Cachorro Completo", "Cachorro completo"),
        ("Caldo Verde", "Caldo verde"),
        ("Bacon Grelhado", "Bacon grelhado"),
        ("Hambúrguer Completo", "Hambúrguer completo"),
        ("Presunto Pata Negra", "Presunto pata negra"),
        ("Cerveja de Pressão 50cl", "Cerveja de pressão 50cl"),
        ("Galão Descafeinado", "Galão descafeinado"),
        ("Casal Garcia", "Casal garcia"),
        ("Toucinho Do Ceu", "Toucinho do Céu"),
        ("Gelado Chocolate", "Gelado chocolate"),
        ("Vinhas Dalém", "Vinhas dalém"),
        ("Pizza Doce", "Pizza doce"),
        ("Cachorro Simples", "Cachorro simples"),
        ("Ponte de Lima", "Ponte de lima"),
        ("Água Pedras Sabores", "Água pedras sabores"),
        ("Doce Da Casa", "Doce da casa"),
        ("Tosta Mista", "Tosta mista"),
        ("Pizza Tradicional", "Pizza tradicional"),
        ("Mateus Rosé", "Mateus rosé"),
        ("Quintela Reserva", "Quintela reserva"),
        ("Talismã Reserva", "Talismã reserva"),
        ("Tapada Dos Monges", "Tapada dos monges"),
        ("Pastel de Feijão", "Pastel de feijão"),
        ("Água Luso 1.5l", "Água luso 1.5l"),
        ("Eclair de Chocolate", "Eclair de chocolate"),
        ("Carioca de Limão", "Carioca de limão"),
        ("Mousse de Chocolate C Chantily", "Mousse de Chocolate com Chantilly"),
        ("Sagres Preta Mini", "Sagres preta mini"),
        ("Castelo Dalba", "Castelo dalba"),
        ("Porca de Murça", "Porca de murça"),
        ("Sopa de Legumes", "Sopa de legumes"),
        ("Alvarinho de Moção", "Alvarinho de moção"),
        ("Meia de Leite Descafeinado", "Meia de leite descafeinado"),
        ("Queijo Saloio", "Queijo saloio"),
        ("Pastel de Côco", "Pastel de côco"),
        ("Presunto Serrano", "Presunto serrano"),
        ("Terra Quente", "Terra quente"),
        ("Pastel de Nata", "Pastel de nata"),
        ("Ninfa Touriga Nacional Syrah", "Ninfa touriga nacional syrah"),
        ("Dona Maria", "Dona maria"),
        ("Ngola Preta", "Ngola preta"),
        ("Mousse de Chocolate Caseira", "Mousse de chocolate caseira"),
        ("Quinta D Aveleda", "Quinta d aveleda"),
        ("Galão Cevada", "Galão cevada"),
        ("Sande Mista", "Sande mista"),
        ("Contos Da Terra", "Contos da terra"),
        ("Frei João", "Frei joão"),
        ("Tarte de amêndoa", "Tarte de Amêndoa"),
        ("Hambúrguer Simples", "Hambúrguer simples"),
        ("Bolo de Arroz", "Bolo de arroz"),
        ("Sagres Preta 33cl", "Sagres preta 33cl"),
        ("Herdade Da Comporta", "Herdade da comporta"),
        ("Copo de Leite", "Copo de leite"),
        ("Croissant Simples", "Croissant simples"),
        ("Água Pedras Salgadas", "Água pedras salgadas"),
        ("Sanduíche de Atum", "Sanduíche de atum"),
        ("Chá Verde", "Chá verde"),
        ("Bolo de Bolacha", "Bolo de bolacha"),
        ("Água Luso 50cl", "Água luso 50cl"),
        ("Tosta de Queijo", "Tosta de queijo"),
        ("Cerveja de Pressão 33cl", "Cerveja de pressão 33cl"),
        ("Croissants de Ovos", "Croissants de ovos"),
        ("Água Castelo Sabores", "Água castelo sabores"),
        ("Chocolate Quente", "Chocolate quente"),
        ("Frize Sabores", "Frize sabores"),
        ("Conde de Villar", "Conde de villar"),
        ("Sande de Fiambre", "Sande de fiambre"),
        ("Sumo Natural", "Sumo natural"),
        ("Red Bull", "Red bull"),
        ("Monte Velho", "Monte velho"),
        ("Maçã Assada", "Maçã assada"),
        ("Quinta Das Apegadas", "Quinta das apegadas"),
        ("Maria Mansa", "Maria mansa"),
        ("Qta Das Bageiras", "Qta das bageiras"),
        ("Palmier Recheado", "Palmier recheado"),
        ("Leite Creme", "Leite creme"),
        ("Grão Vasco", "Grão vasco"),
        ("Queijo Azeitão", "Queijo azeitão"),
        ("Sopa de Marisco", "Sopa de marisco"),
        ("Pão de Alho", "Pão de alho"),
        ("Torta de Noz", "Torta de noz"),
        ("Toucinho do céu", "Toucinho do Céu"),
        ("Freixenet Bruto", "Freixenet bruto"),
        ("Cutty Sark", "Cutty sark"),
        ("Ovos Estrelados C Bacon", "Ovos estrelados com bacon"),
        ("Ballantines 12 Anos", "Ballantines 12 anos"),
        ("Costeleta de Novilho Grelhada", "Costeleta de novilho grelhada"),
        ("Aliança Particular", "Aliança particular"),
        ("Murganheira Meio Seco", "Murganheira meio seco"),
        ("Borlido Meio Seco", "Borlido meio seco"),
        ("Bacalhau À Lagareiro", "Bacalhau à lagareiro"),
        ("Lombinhos C Cogumelos", "Lombinhos com cogumelos"),
        ("Sardinha Assada", "Sardinha assada"),
        ("Arroz de Marisco", "Arroz de marisco"),
        ("Bacalhau Recheado", "Bacalhau recheado"),
        ("Secretos de Porco Preto", "Secretos de porco preto"),
        ("Montanha Real", "Montanha real"),
        ("Campolargo Rosé", "Campolargo rosé"),
        ("Salada de Atum com Mayonese", "Salada de atum com mayonese"),
        ("Jack Daniels Old N 7", "Jack daniels old n 7"),
        ("Brandy Macieira", "Brandy macieira"),
        ("Castelar Baga", "Castelar baga"),
        ("Bife Na Pedra", "Bife na pedra"),
        ("Massada de Marisco", "Massada de marisco"),
        ("Brandy Croft", "Brandy croft"),
        ("Anis Doce Domuz", "Anis doce domuz"),
        ("Menu de Dieta", "Menu de dieta"),
        ("Bitoque de Vaca", "Bitoque de vaca"),
        ("Entrecosto Porco Grelhado", "Entrecosto porco grelhado"),
        ("Aplauso Bical", "Aplauso bical"),
        ("Agua Tonica Schweppes", "Agua tonica schweppes"),
        ("Primavera Baga", "Primavera baga"),
        ("Carne Porco Alentejana", "Carne porco alentejana"),
        ("Cachaca Milbar", "Cachaca milbar"),
        ("Legumes Salteados", "Legumes salteados"),
        ("Salmão Grelhado", "Salmão grelhado"),
        ("Escalopes de Peru", "Escalopes de peru"),
        ("Aguardente Aldeia Velha", "Aguardente aldeia velha"),
        ("Bacalhau À Casa", "Bacalhau à casa"),
        ("Bacalhau Assado Na Brasa", "Bacalhau assado na brasa"),
        ("Lambrusco Rosé", "Lambrusco rosé"),
        ("Licor Baileys", "Licor baileys"),
        ("Montanha Chardonnay", "Montanha chardonnay"),
        ("Feijoada de Marisco", "Feijoada de marisco"),
        ("Arroz de Lulas", "Arroz de lulas"),
        ("Omolete de Presunto", "Omolete de presunto"),
        ("Frango de Caril", "Frango de caril"),
        ("Gin N1 The London", "Gin N1 the london"),
        ("Frango de Churrasco", "Frango de churrasco"),
        ("Pescada Cozida", "Pescada cozida"),
        ("Bife de Pimenta", "Bife de pimenta"),
        ("Polvo À Lagareiro", "Polvo à lagareiro"),
        ("Carapau Grelhado", "Carapau grelhado"),
        ("Martini Rosso", "Martini rosso"),
        ("Batata Frita", "Batata frita"),
        ("Bacalhau À Brás", "Bacalhau à brás"),
        ("Bife Cortador", "Bife cortador"),
        ("São Domingos Rosé Bruto", "São domingos rosé bruto"),
        ("Terras Do Demo Bruto", "Terras do demo bruto"),
        ("Carregar Cartão de Cliente", "Carregar cartão de cliente"),
        ("Espetada de Tamboril C Camarão", "Espetada de tamboril com camarão"),
        ("Marquês de Marialva Bruto Reserva", "Marquês de marialva bruto reserva"),
        ("Arroz de Polvo", "Arroz de polvo"),
        ("Ataíde Semedo Cuvée Bruto 2011", "Ataíde semedo cuvée bruto 2011"),
        ("Paté de Sardinha", "Paté de sardinha"),
        ("Ementa Turistica", "Ementa turistica"),
        ("Menu de Peixe", "Menu de peixe"),
        ("Asti Gancia Doce", "Asti gancia doce"),
        ("Encontro Bruto 2010", "Encontro bruto 2010"),
        ("Vodka Eristoff", "Vodka eristoff"),
        ("Amêndoa Amarga", "Amêndoa amarga"),
        ("Camarão C Molho Vinagrete", "Camarão com molho vinagrete"),
        ("Batata Cozida", "Batata cozida"),
        ("Cocktail de Camarão", "Cocktail de camarão"),
        ("Menu de Carne", "Menu de carne"),
        ("Licor Sheridans", "Licor sheridans"),
        ("Espetada Mista", "Espetada mista"),
        ("Licor Malibu", "Licor malibu"),
        ("Constantino Brandy", "Constantino brandy"),
        ("Omolete Simples", "Omolete simples"),
        ("Caves São João Reserva", "Caves são joão reserva"),
        ("Omolete Queijo e Fiambre", "Omolete queijo e fiambre"),
        ("Bitoque de Porco", "Bitoque de porco"),
        ("Bacardi Pina Colada", "Bacardi pina colada"),
        ("Quinta Do Ortigão Baga", "Quinta do ortigão baga"),
        ("Menu Vegetariano", "Menu vegetariano"),
        ("Bacalhau À Gomes Sá", "Bacalhau à gomes sá"),
        ("Arroz de Tamboril", "Arroz de tamboril"),
        ("Martini Bianco", "Martini bianco"),
        ("Bacalhau Na Cataplana", "Bacalhau na cataplana"),
        ("Lambrusco Tinto", "Lambrusco tinto"),
        ("Mata Fidalga Rosé", "Mata fidalga rosé"),
        ("Costeleta de Porco Grelhada", "Costeleta de porco grelhada"),
        ("Vodka Smirnoff", "Vodka smirnoff"),
        ("Grelhada Mista", "Grelhada mista"),
        ("Lulas Recheadas", "Lulas recheadas"),
        ("Frango no Forno", "Frango no forno"),
        ("Toucinho do Ceu", "Toucinho do Céu"),
        ("Tarte De Amendoa", "Tarte de Amêndoa"),
        ("Mousse De Chocolate C Chantily", "Mousse de Chocolate com Chantilly"),
        ("Hamburguer No Prato", "Hambúrguer no prato"),
        ("Hamburguer Completo", "Hambúrguer completo"),
        ("Hamburguer Simples", "Hambúrguer simples"),
    ];

    public static readonly (string Old, string New)[] ArticleSubfamilyDesignations =
    [
        ("Café E Leite", "Café e leite"),
        ("Sandes E Tostas", "Sandes e tostas"),
        ("Vinhos Brancos", "Vinhos brancos"),
        ("Vinhos Verdes", "Vinhos verdes"),
        ("Outras Caf", "Outros de cafetaria"),
        ("Vinhos Tintos", "Vinhos tintos"),
        ("Ref Rápidas", "Refeições rápidas"),
        ("Outras cafetaria", "Outros de cafetaria"),
        ("Outras Ent", "Outras entradas"),
        ("Cartão De Cliente", "Cartão de cliente"),
    ];

    public static IEnumerable<string> ForSqlite() => Build(SqlDialect.Sqlite);

    public static IEnumerable<string> ForMySql() => Build(SqlDialect.MySql);

    public static IEnumerable<string> ForSqlServer() => Build(SqlDialect.SqlServer);

    private enum SqlDialect
    {
        Sqlite,
        MySql,
        SqlServer
    }

    private static IEnumerable<string> Build(SqlDialect dialect)
    {
        yield return DeleteDuplicateTransport(dialect);

        foreach (var sql in ClearSmtpExamples(dialect))
        {
            yield return sql;
        }

        yield return UpdateExact(dialect, "PaymentMethods", "Designation", "Dinheiro Eletrônico", "Dinheiro Eletrónico");
        yield return UpdateExact(dialect, "PermissionItems", "Designation", "__designation[4040]__", "Relatório :: Stock por valorização de artigo");

        foreach (var (oldValue, newValue) in ArticleDesignations)
        {
            yield return UpdateExact(dialect, "Articles", "Designation", oldValue, newValue);
        }

        foreach (var (oldValue, newValue) in ArticleSubfamilyDesignations)
        {
            yield return UpdateExact(dialect, "ArticleSubfamilies", "Designation", oldValue, newValue);
        }
    }

    private static IEnumerable<string> ClearSmtpExamples(SqlDialect dialect)
    {
        string[] userExamples = ["YOUR-EMAIL@gmail.com", "exemplo@empresa.pt"];
        string[] passwordExamples = ["YOUR-PASSWORD"];

        foreach (var example in userExamples)
        {
            yield return UpdateTokenValue(dialect, "SEND_MAIL_SMTP_USERNAME", example, string.Empty);
        }

        foreach (var example in passwordExamples)
        {
            yield return UpdateTokenValue(dialect, "SEND_MAIL_SMTP_PASSWORD", example, string.Empty);
        }
    }

    private static string DeleteDuplicateTransport(SqlDialect dialect) =>
        dialect switch
        {
            SqlDialect.Sqlite =>
                """
DELETE FROM PreferenceParameters
WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  AND rowid NOT IN (
    SELECT MIN(rowid) FROM PreferenceParameters
    WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  );
""",
            SqlDialect.MySql =>
                """
DELETE FROM PreferenceParameters
WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  AND Id NOT IN (
    SELECT Id FROM (
      SELECT Id
      FROM PreferenceParameters
      WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
      ORDER BY CreatedAt ASC, Id ASC
      LIMIT 1
    ) AS KeepTransport
  );
""",
            _ =>
                """
WITH RankedTransport AS (
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY CreatedAt ASC, Id ASC) AS RowNumber
    FROM PreferenceParameters
    WHERE Token = N'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
)
DELETE FROM PreferenceParameters
WHERE Token = N'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  AND Id IN (SELECT Id FROM RankedTransport WHERE RowNumber > 1);
"""
        };

    private static string UpdateTokenValue(SqlDialect dialect, string token, string oldValue, string newValue)
    {
        var tokenLiteral = Literal(dialect, token);
        var oldLiteral = Literal(dialect, oldValue);
        var newLiteral = Literal(dialect, newValue);
        return $"UPDATE PreferenceParameters SET Value = {newLiteral} WHERE Token = {tokenLiteral} AND Value = {oldLiteral};";
    }

    private static string UpdateExact(SqlDialect dialect, string table, string column, string oldValue, string newValue)
    {
        var oldLiteral = Literal(dialect, oldValue);
        var newLiteral = Literal(dialect, newValue);
        return $"UPDATE {table} SET {column} = {newLiteral} WHERE {column} = {oldLiteral};";
    }

    private static string Literal(SqlDialect dialect, string value)
    {
        var escaped = value.Replace("'", "''");
        return dialect == SqlDialect.SqlServer ? $"N'{escaped}'" : $"'{escaped}'";
    }
}
