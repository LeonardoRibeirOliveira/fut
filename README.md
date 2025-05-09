# FHIRUT - FHIR Unit Test Suite  
**Arquitetura Simplificada & Visão Geral**  

## 📌 **Visão Geral**  
O **FHIRUT** é uma ferramenta para automação de testes de conformidade de recursos FHIR, inspirada no conceito do JUnit. Ele:  
- Valida instâncias FHIR contra perfis/IGs usando o `validator_cli.jar` oficial.  
- Compara resultados com expectativas definidas em arquivos YAML.  
- Gera relatórios detalhados (HTML/JSON).  

---

## 🏗️ **Arquitetura**  
### 1. **Frontend (Angular)**  
- **Função**: Interface interativa para usuários.  
- **Componentes Principais**:  
  - **Dashboard**: Gráficos de resumo (testes aprovados/falhados).  
  - **Visualizador de Testes**: Detalhes de casos de teste, diferenças entre resultados esperados e obtidos.  
  - **Upload de Arquivos**: Envio de arquivos YAML/JSON para validação.  

### 2. **Backend (.NET)**  
- **Função**: Processamento pesado e integração com validador FHIR.  
- **Módulos Principais**:  
  - **Test Runner**:  
    - Executa `validator_cli.jar` em paralelo para múltiplos testes.  
    - Gerencia timeouts e erros.  
  - **Comparador de Resultados**:  
    - Analisa `OperationOutcome` do validador vs. `expected_results` do YAML.  
    - Suporta FHIRPath para validação de invariantes.  
  - **Gerador de Relatórios**:  
    - Cria saídas em HTML (visual) e JSON (máquina).  

### 3. **Validador FHIR Externo**  
- **validator_cli.jar**: Ferramenta oficial da HL7 chamada pelo backend via linha de comando.  

---

## 🔄 **Fluxo de Execução**  
1. **Entrada**: Usuário envia arquivos YAML (definições de teste) via frontend.  
2. **Processamento**:  
   - Backend lê YAML, localiza instâncias FHIR (por convenção ou caminho explícito).  
   - Executa validação via `validator_cli.jar` com parâmetros do contexto (IGs, perfis).  
3. **Saída**:  
   - Relatório HTML/JSON com comparação lado a lado (esperado vs. obtido).  
   - Estatísticas agregadas (tempo de execução, taxa de sucesso).  

---

## 🛠️ **Tecnologias-Chave**  
| Componente       | Tecnologias                                                                 |  
|------------------|----------------------------------------------------------------------------|  
| **Frontend**     | Angular, TypeScript, HTML/CSS, Tailwind CSS, Chart.js (gráficos)                         |  
| **Backend**      | .NET 8+, ASP.NET Core, YamlDotNet, Hl7.Fhir.Serialization                 |  
| **Validação**    | validator_cli.jar (Java)                                                   |  
| **Relatórios**   | HTML Dinâmico (Angular), JSON Schema                                       |  

---

## ✅ **Por Que Esta Arquitetura?**  
- **Angular**: Ideal para interfaces ricas e dinâmicas (ex.: realce de diferenças em relatórios).  
- **.NET**: Robustez para processamento paralelo, chamadas de sistema (Java), e manipulação de FHIR/YAML.  
- **Modular**: Facilita substituição de componentes (ex.: trocar validador oficial por outro).  

> **Status Atual**: Backend funcional com geração de relatórios, frontend em evolução com protótipos de dashboard e upload. 

>---

## 🔮 **Próximos Passos**

### 🔹 Backend (.NET)
- [ ] Implementar endpoint para execução individual de testes.
- [ ] Melhorar tratamento de erros do `validator_cli.jar` com mensagens claras.
- [ ] Adicionar testes automatizados (unidade e integração).
- [ ] Permitir configuração de múltiplos IGs e perfis via API.

### 🔹 Frontend (Angular)
- [ ] Finalizar dashboard com gráficos e contadores.
- [ ] Adicionar tela de visualização de logs e relatórios.
- [ ] Implementar validação e preview dos arquivos YAML no upload.
- [ ] Adicionar filtro e busca por status dos testes.

### 🔹 Documentação
- [ ] Criar exemplos completos de arquivos YAML.
- [ ] Documentar a API com Swagger (ou alternativa).
- [ ] Criar tutorial passo a passo para novos usuários.

 