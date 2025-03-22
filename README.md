# Tecnologias
- ASP.NET Core API/MVC/Blazor: Utilizado para gerenciar os casos de teste, suítes de teste e a execução dos testes. Serve como a base para a comunicação entre os componentes e para a criação de uma interface de usuário. Além disso, Blazor para a criação de relatórios dinâmicos e interativos, com gráficos e visualização de resultados dos testes.

- Entity Framework Core: Usado para persistir dados sobre os casos de teste, suítes e resultados em um banco de dados relacional (SQL Server, SQLite, etc.).

- XUnit/NUnit/MSTest: Frameworks de teste em .NET para organizar e executar os testes de forma estruturada.

- CLI (Command Line Interface): Para executar o validador FHIR, como validator_cli, a partir do código ASP.NET Core.

- Task Parallel Library (TPL): Para gerenciar a execução paralela de testes, se necessário, durante a execução dos testes.

- JSON Diff (JsonDiffPatch.Net ou Newtonsoft.Json): Para comparar os resultados obtidos com os resultados esperados, especialmente se os dados forem em formato JSON.
