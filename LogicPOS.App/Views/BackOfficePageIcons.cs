namespace LogicPOS.App.Views;

/// <summary>Maps BackOffice page titles to the same menu SVG used in the sidebar.</summary>
internal static class BackOfficePageIcons
{
    private const string AssetRoot = "avares://LogicPOS.App/Assets/Images/BackOffice/";

    private static readonly Dictionary<string, string> ByTitle = new(StringComparer.Ordinal)
    {
        ["Documentos"] = "Menu/menu_dashboard_documentos_b.svg",
        ["Novo Doc."] = "Menu/menu_dashboard_novo_documento.svg",
        ["Emissão Recibos"] = "Menu/menu_dashboard_emissao_recibos.svg",
        ["Recibos"] = "Menu/menu_dashboard_recibos.svg",
        ["Conta.Corr."] = "Menu/menu_dashboard_conta_corrente.svg",
        ["Sessões de Trab."] = "Menu/menu_dashboard_documentos_b.svg",
        ["Relatórios"] = "Menu/menu_relatorios_relatorios.svg",
        ["Artigos"] = "Menu/menu_artigos_artigos.svg",
        ["Famílias"] = "Menu/menu_artigos_familas.svg",
        ["Subfamílias"] = "Menu/menu_artigos_subfamilias.svg",
        ["Tipo de artigos"] = "Menu/menu_artigos_tipos_artigo.svg",
        ["Classe do artigo"] = "Menu/menu_artigos_artigos.svg",
        ["Tipo de Preço"] = "Menu/menu_artigos_tipos_preco.svg",
        ["Gestão de Stocks"] = "Menu/menu_artigos_gestao_stocks.svg",
        ["Importar Artigos"] = "Menu/menu_importacao_importar_artigos.svg",
        ["Exportar Artigos"] = "Menu/menu_exportacao_exportar_artigos.svg",
        ["Informação fiscal"] = "Menu/menu_informacoes_fiscais.svg",
        ["Abertura de ano fiscal"] = "Menu/menu_informacoes_fiscais_anos_fiscais.svg",
        ["Séries"] = "Menu/menu_informacoes_fiscais_series_documentos.svg",
        ["Tipo de documento"] = "Menu/menu_informacoes_fiscais_tipos_documentos.svg",
        ["Taxas de imposto"] = "Menu/menu_informacoes_fiscais_taxas_iva.svg",
        ["Motivo de isenção de IVA"] = "Menu/menu_informacoes_fiscais_movimentos_isencao_iva.svg",
        ["Cond. de Pagamento"] = "Menu/menu_informacoes_fiscais_condicoes_pagamento.svg",
        ["Métodos de pagamento"] = "Menu/menu_informacoes_fiscais_metodo_pagamento.svg",
        ["Clientes"] = "Menu/menu_clientes.svg",
        ["Tipo de clientes"] = "Menu/menu_clientes_tipo_clientes.svg",
        ["Grupo de descontos"] = "Menu/menu_clientes_grupo_desconto.svg",
        ["Importar Clientes"] = "Menu/menu_importacao_importar_clientes.svg",
        ["Exportar Clientes"] = "Menu/menu_exportacao_exportar_clientes.svg",
        ["Utilizadores"] = "Menu/menu_utilizadores.svg",
        ["Permissões"] = "Menu/menu_utilizadores_permicoes.svg",
        ["Grupo de comissões"] = "Menu/menu_utilizadores_grupo_comicao.svg",
        ["Dispositivos"] = "Menu/menu_dispositivos.svg",
        ["Tipos de impressora"] = "Menu/menu_dispositivos_tipos_impressoras.svg",
        ["Impressoras"] = "Menu/menu_dispositivos_impressoras.svg",
        ["Dispositivos de Entrada"] = "Menu/menu_dispositivos_leitores.svg",
        ["Display de Cliente"] = "Menu/menu_dispositivos_display.svg",
        ["Balanças"] = "Menu/menu_dispositivos_balancas.svg",
        ["Outras Tabelas"] = "Menu/menu_outras_tabelas.svg",
        ["País"] = "Menu/menu_outras_tabelas_menu_dispositivos_paises.svg",
        ["Moeda"] = "Menu/menu_outras_tabelas_moedas.svg",
        ["Locais"] = "Menu/menu_outras_tabelas_locais.svg",
        ["Mesas"] = "Menu/menu_outras_tabelas_mesas.svg",
        ["Tipo de Movimento"] = "Menu/menu_outras_tabelas_tipo_movimento.svg",
        ["Unidades de medida"] = "Menu/menu_outras_tabelas_unidade_medida.svg",
        ["Unidades de tamanho"] = "Menu/menu_outras_tabelas_unidade_tamanho.svg",
        ["Feriados"] = "Menu/menu_outras_tabelas_feriados.svg",
        ["Armazém"] = "Menu/menu_outras_tabelas_armazens.svg",
        ["Configuração"] = "Menu/menu_configuracoes.svg",
        ["Parâmetros da Empresa"] = "Menu/menu_configuracoes_preferencias_empresa.svg",
        ["Parâmetros de Sistema"] = "Menu/menu_configuracoes_preferencias_sistema.svg",
        ["Terminais"] = "Menu/menu_configuracoes_terminais_b.svg",
        ["SAFT"] = "Menu/menu_exportacao.svg",
        ["SAF-T ano"] = "Menu/menu_exportacao_saft_anual.svg",
        ["SAF-T último mês"] = "Menu/menu_exportacao_saft_ultimo_mes.svg",
        ["SAF-T período"] = "Menu/menu_exportacao_saft_personalizado.svg",
        ["AGT"] = "Menu/menu_exportacao.svg",
        ["AGT ano"] = "Menu/menu_exportacao_saft_anual.svg",
        ["AGT último mês"] = "Menu/menu_exportacao_saft_ultimo_mes.svg",
        ["AGT período"] = "Menu/menu_exportacao_saft_personalizado.svg",
        ["Séries AGT"] = "Menu/menu_informacoes_fiscais_series_documentos.svg",
        ["Documentos AGT"] = "Menu/menu_informacoes_fiscais_tipos_documentos.svg",
        ["Sistema"] = "Menu/menu_sistema.svg",
        ["Notificações"] = "Menu/menu_sistema_notificacoes.svg",
        ["Registro de alterações (Changelog)"] = "Menu/menu_sistema_change_logs.svg",
        ["Backup DB"] = "Menu/menu_sistema_backup.svg",
        ["Restaurar DB"] = "Menu/menu_sistema_restauro.svg",
        ["Restaurar de ficheiro"] = "Menu/menu_sistema_restauro.svg",
        ["Atualizar"] = "Menu/menu_sistema.svg",
        ["Sair da Sessão"] = "Menu/menu_sistema_logout.svg",
        ["Motivos"] = "Menu/menu_informacoes_fiscais_movimentos_isencao_iva.svg"
    };

    public static void Register(string title, string? iconRelative, string? fallbackRelative = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        var path = string.IsNullOrWhiteSpace(iconRelative) ? fallbackRelative : iconRelative;
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        ByTitle[title.Trim()] = path;
    }

    public static string? UriFor(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        if (ByTitle.TryGetValue(title.Trim(), out var relative) && !string.IsNullOrWhiteSpace(relative))
        {
            // Page headers are on a light surface — prefer the _b variant when registered without it.
            return AssetRoot + PreferLightSurfaceIcon(relative);
        }

        return null;
    }

    private static string PreferLightSurfaceIcon(string relative)
    {
        if (relative.EndsWith("_b.svg", StringComparison.OrdinalIgnoreCase))
            return relative;

        if (relative.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            return string.Concat(relative.AsSpan(0, relative.Length - 4), "_b.svg");

        return relative;
    }

    public static void Apply(Avalonia.Svg.Skia.Svg? icon, string? title)
    {
        if (icon is null)
        {
            return;
        }

        var uri = UriFor(title);
        if (uri is null)
        {
            icon.IsVisible = false;
            icon.Path = string.Empty;
            return;
        }

        icon.Path = uri;
        icon.IsVisible = true;
    }
}
