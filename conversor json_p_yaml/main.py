import json
import yaml

def json_to_yaml(json_data):
    # Carregar o JSON
    parsed_json = json.loads(json_data)

    # Converter para YAML
    yaml_data = yaml.dump(parsed_json, default_flow_style=False, allow_unicode=True)

    return yaml_data

# Exemplo de JSON
json_data = '''{
  "resourceType": "ActivityDefinition",
  "id": "administer-zika-virus-exposure-assessment",
  "text": {
    "status": "generated",
    "div": "<div xmlns=\\"http://www.w3.org/1999/xhtml\\">\\n         <table class=\\"grid dict\\">\\n            <tr>\\n               <td>\\n                  <b>Id: </b>\\n               </td>\\n            </tr>\\n            <tr>\\n               <td style=\\"padding-left: 25px; padding-right: 25px;\\">ActivityDefinition/administer-zika-virus-exposure-assessment</td>\\n            </tr>\\n         </table>\\n         <p/>\\n         <table class=\\"grid dict\\">\\n            <tr>\\n               <td>\\n                  <b>Status: </b>\\n               </td>\\n            </tr>\\n            <tr>\\n               <td style=\\"padding-left: 25px; padding-right: 25px;\\">draft</td>\\n            </tr>\\n         </table>\\n         <p/>\\n         <table class=\\"grid dict\\">\\n            <tr>\\n               <td>\\n                  <b>Description: </b>\\n               </td>\\n            </tr>\\n            <tr>\\n               <td style=\\"padding-left: 25px; padding-right: 25px;\\">Administer Zika Virus Exposure Assessment</td>\\n            </tr>\\n         </table>\\n         <p/>\\n         <table class=\\"grid dict\\">\\n            <tr>\\n               <td>\\n                  <b>Category: </b>\\n               </td>\\n            </tr>\\n            <tr>\\n               <td style=\\"padding-left: 25px; padding-right: 25px;\\">procedure</td>\\n            </tr>\\n         </table>\\n         <p/>\\n         <table class=\\"grid dict\\">\\n            <tr>\\n               <td>\\n                  <b>Code: </b>\\n               </td>\\n            </tr>\\n            <tr>\\n               <td style=\\"padding-right: 25px;\\">\\n                  <span>\\n                     <span>\\n                        <span style=\\"padding-left: 25px;\\">\\n                           <b>system: </b>\\n                           <span>http://example.org/questionnaires</span>\\n                           <br/>\\n                        </span>\\n                        <span style=\\"padding-left: 25px;\\">\\n                           <b>code: </b>\\n                           <span>zika-virus-exposure-assessment</span>\\n                           <br/>\\n                        </span>\\n                     </span>\\n                  </span>\\n               </td>\\n            </tr>\\n         </table>\\n         <p/>\\n         <table class=\\"grid dict\\">\\n            <tr>\\n               <td>\\n                  <b>Participant: </b>\\n               </td>\\n            </tr>\\n            <tr style=\\"vertical-align: top;\\">\\n               <td style=\\"padding-left: 25px; padding-right: 25px;\\">practitioner</td>\\n            </tr>\\n         </table>\\n      </div>"
  },
  "url": "http://example.org/ActivityDefinition/administer-zika-virus-exposure-assessment",
  "status": "draft",
  "description": "Administer Zika Virus Exposure Assessment",
  "useContext": [
    {
      "code": {
        "system": "http://terminology.hl7.org/CodeSystem/usage-context-type",
        "code": "age"
      },
      "valueRange": {
        "low": {
          "value": 12,
          "unit": "a"
        }
      }
    }
  ],
  "relatedArtifact": [
    {
      "type": "derived-from",
      "url": "https://www.cdc.gov/zika/hc-providers/pregnant-woman.html"
    },
    {
      "type": "depends-on",
      "resource": "Questionnaire/zika-virus-exposure-assessment"
    }
  ],
  "library": [
    "Library/zika-virus-intervention-logic"
  ],
  "kind": "ServiceRequest",
  "code": {
    "coding": [
      {
        "system": "http://example.org/questionnaires",
        "code": "zika-virus-exposure-assessment"
      }
    ]
  },
  "timingTiming": {
    "_event": [
      {
        "extension": [
          {
            "url": "http://hl7.org/fhir/StructureDefinition/cqf-expression",
            "valueExpression": {
              "language": "text/cql",
              "expression": "Now()"
            }
          }
        ]
      }
    ]
  },
  "participant": [
    {
      "type": "practitioner"
    }
  ]
}'''

# Chamar a função
yaml_output = json_to_yaml(json_data)

# Exibir o YAML gerado
print(yaml_output)
