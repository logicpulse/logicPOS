Status: implemented (tickets 11–18 and decisions 02–08). Ticket 04 was already the mixed-payment behaviour.

## Problem Statement

Quem usa a edição LogicPulse vê, no primeiro arranque, a Community descrita como faturação local sem certificação AT. A Community dessa edição tem certificação. O recibo nasce em Cartão de Crédito, quando o método por omissão é Numerário. Vários PDFs trazem o título de outro relatório, e «Vendas por taxa IVA e tipo de artigo» agrupa por tipo de documento. O sumário de «Vendas por condição de pagamento» repete uma linha por artigo. Um relatório sem dados ainda pode sair em inglês. «Ver relatório completo» no Top 10 artigos abre a lista de Artigos. O modo do cartão do cliente aparece como «Debit». Sistema → Atualizar pergunta pelo download antes de saber se há versão nova.

## Solution

A Community da edição LogicPulse passa a dizer que a faturação local tem certificação. A edição pública mantém a frase de faturação local sem certificação AT integrada. O recibo selecciona Numerário. O título de cada PDF passa a ser o nome do botão que o abriu, e «Vendas por taxa IVA e tipo de artigo» agrupa por tipo de artigo. O sumário de vendas por condição de pagamento agrupa por condição e mostra os totais das linhas, sem artigo. Um relatório vazio que ainda saía em inglês passa a português. O atalho do Top 10 artigos abre «Total vendido por artigo». O modo do cartão mostra Débito e Crédito. A pergunta de atualização só aparece depois de haver uma versão mais nova, e diz qual é.

## User Stories

1. As a person starting the LogicPulse edition, I want the Community choice to say that local invoicing includes certification, so that I do not think the installed app is uncertified.
2. As a person starting the public edition, I want the Community choice to keep saying that local invoicing has no integrated AT certification, so that the public edition is not described as certified.
3. As a person comparing Community, Business and Cloud on the LogicPulse edition, I want Business and Cloud to remain the choices that add the LogicPulse certification on top of the certified Community posto, so that the three choices stay distinct.
4. As a person reading the Community feature list on the LogicPulse edition, I want the line about invoicing and PDF documents on the posto to include certification, so that it does not repeat «sem certificação AT integrada».
5. As a person issuing a receipt, I want Numerário selected by default, so that I do not have to change the method when the customer paid cash.
6. As a person issuing a receipt in a database whose methods were renamed, I want Numerário chosen by its role as cash, so that a custom label does not make the default fall through to the first method in the list.
7. As a person issuing a receipt when Numerário is absent, I want a defined fallback that is not a silent credit-card default, so that the missing cash method is visible.
8. As a person opening «Vendas por mesa (Detalhado)», I want the PDF title to be that name, so that I can tell it from «Vendas por zona (Detalhado)».
9. As a person opening «Vendas por zona (Detalhado)», I want that PDF to keep a zone title, so that fixing the table report does not rename the zone report.
10. As a person opening «Vendas por taxa IVA e tipo de artigo», I want the PDF title to be that name, so that it is not titled «Vendas por Taxa IVA».
11. As a person reading that PDF, I want rows grouped by article type, so that the report matches the button.
12. As a person reading that PDF, I want the group column to name the article type, so that it does not say «Tipo Documento».
13. As a person opening «Vendas por taxa IVA e classe de artigo», I want its title and grouping to stay about article class, so that the article-type fix does not collapse the two reports.
14. As a person opening «Ganho por artigo», I want the PDF title to be that name, so that it is not titled «Stock por valorização de artigo».
15. As a person opening «Auditoria do sistema», I want the PDF title to be that name, so that it is not titled «Tabelas de Auditoria».
16. As a person opening any other report whose button name and PDF title already match, I want that title left as it is, so that this correction does not rename reports that are already right.
17. As a person opening «Vendas por condição de pagamento» on «Financeiros (Sumário)», I want one group per payment condition, so that I can see the condition before the figures.
18. As a person reading that summary, I want the totals of the lines in each condition, so that I see a summary rather than every article.
19. As a person reading that summary, I want no article code, designation or quantity, so that it is not the detailed report.
20. As a person opening «Vendas por condição de pagamento (Detalhado)», I want to keep one row per article, so that the summary change does not remove the detailed report.
21. As a person generating a report that has no rows and still produces the English page, I want that page in Portuguese, so that I am not told «No Content. Check the Filters».
22. As a person generating «Stock por artigo», «Stock por fornecedor» or «Ganho por artigo» with no rows, I want the Portuguese alert already present in the local work to remain, so that those three reports are not switched back to a page.
23. As a person using Sistema → Atualizar when a newer version exists, I want the question to name that version before I agree to download and restart, so that I know what will be installed.
24. As a person using Sistema → Atualizar when nothing newer exists, I want no download question, so that I am not asked to restart for an update that is not there.
25. As a person who already compared version numbers elsewhere, I want this change to use that comparison, so that «newer» means the same thing it already means.
26. As a person on the dashboard looking at «Top 10 artigos», I want «Ver relatório completo» to open «Total vendido por artigo», so that I see sales by article.
27. As a person following that link, I want the report whose PDF is «Total de Artigo Vendido», so that the button and the document are the same report.
28. As a person following that link, I want to land in reports rather than in the Artigos list, so that I do not have to find the report myself.
29. As a person editing a customer, on Comercial, I want «Modo do cartão» to show Débito and Crédito, so that the field is not the English word «Debit».
30. As a person saving that customer, I want the stored mode to stay Debit or Credit, so that the Portuguese label does not change the value.
31. As a person editing a table whose status labels are already Livre, Ocupada and Reservada on the unmerged work, I want this spec to leave those labels alone, so that the card-mode fix does not redo the table status.

## Implementation Decisions

- Two Community texts, chosen by edition. The public edition states local invoicing without integrated AT certification. The LogicPulse edition states that Community includes certification, because the fiscal module ships with the app. Business and Cloud on the LogicPulse edition stay certified choices. The feature line under Community on the LogicPulse edition does not say «sem certificação AT integrada».
- Receipt default is Numerário, identified as the cash method, not as whichever method is first. If Numerário is missing, the emission does not quietly pretend Cartão de Crédito was the announced default; the operator sees that cash is unavailable.
- PDF title follows the menu name for the reports called out above. The zone report keeps its own title. Reports whose title already matches the menu are unchanged.
- «Vendas por taxa IVA e tipo de artigo» groups by article type. The column names that group. It does not group by document type. The sibling report by article class is unchanged.
- «Vendas por condição de pagamento» in the summary group is one block per payment condition. Each block shows the totals of the lines in that condition. It has no article code, designation or quantity. The detailed report of the same name keeps one row per article.
- An empty report that still renders the English page renders a Portuguese empty result instead. The three stock reports that already show the Portuguese no-records alert stay on that alert.
- Sistema → Atualizar asks to download and restart only after a newer version is known, and the question names that version. Absence of a newer version produces no download question. The existing version comparison is reused; this spec does not redefine what counts as newer.
- «Ver relatório completo» on «Top 10 artigos» opens the report «Total vendido por artigo» (the document titled «Total de Artigo Vendido»). It does not open the Artigos list.
- Card mode labels are Débito for Debit and Crédito for Credit. The stored value remains the existing mode.

## Testing Decisions

- Test the observable result, not the window and not a seam that already exists. Existing version comparison, installer planning, seed migrations, listing tips and table-status labels are not part of this spec.
- One new behavioral check per result below is enough. No Avalonia window test.
- Community text: the LogicPulse wording includes certification; the public wording states local invoicing without integrated AT certification.
- Receipt: when Numerário is among the methods, it is the default; when it is absent, the default is not silently Cartão de Crédito.
- Report document: the table-detailed title is the table menu name; the zone report keeps a zone title; the VAT-and-article-type document is titled and grouped by article type; the gain-by-article and system-audit titles match their menu names; the payment-condition summary contains one group per condition and no article code, designation or quantity; the detailed payment-condition document still has an article row.
- Empty report: a report that used the English empty page now yields Portuguese text. The three stock reports already on the Portuguese alert are not asserted again.
- Update offer: no question when nothing is newer; when something is newer, the question includes that version.
- Dashboard link: the Top 10 articles action names «Total vendido por artigo».
- Card mode: the visible labels are Débito and Crédito while the values stay Debit and Credit.
- Prior art is the existing Core facts that assert one result of a pure choice. These checks are new facts for results that those facts do not cover. There is no existing report-document suite to extend.

## Out of Scope

- Anything already present on `dev`, on the local stock-report diff, or on the 2026-11-23 branches that are not merged: the duplicate Portugal preference key, the seed-text migration for databases that already exist, the payment-button renames and their tips, the copy already corrected there (including «série activa» and «deseja Imprimir»), the installer launch without administrator rights, and the table status labels Livre, Ocupada and Reservada.
- The three stock reports that already show a Portuguese alert when they have no rows, and the filter layout and period shortcuts already in the local report work.
- Decisions not made: whether MB Way replaced Multibanco or they are two methods; whether an update rewrites seed texts on an existing database beyond the migration already written; whether Conta Corrente may be a portion of a mixed payment; what Gerar does with no customer or no article beyond the local change already made; what the default report period is; what the Reemb. Volta tip must say beyond the wording already on the unmerged branch; whether the cloud work-session list shows the filtered period or only open terminals.
- Calling the public edition «Community».
- Writing the code in this spec.

## Further Notes

- The boundary «already there» is the public `dev` tip, the uncommitted stock-report work in this checkout, and the unmerged branches fetched on 2026-11-23 (`fix/dados-2.0.35-r2`, `fix/ux-2.0.35-r3`, `fix/instalador-2.0.35`, `fix/grafia-ativa-internal`, and their siblings). None of those branches are merged.
- «Vendas por artigo Resumido» is the menu name «Total vendido por artigo». The PDF title of that report is «Total de Artigo Vendido».
- Glossary already locks Numerário as the receipt default, and certification on every LogicPulse choice, including Community. This spec applies that; it does not reopen it.
