# Trabalho-INF325
Trabalho de conclusão da disciplina - Modelagem e Projeto de Bancos de Dados (2020)

## Equipe 6

- Agner Esteves Ballejo 
- Ian Poli Tavares 
- José Eduardo Porte
- Mateus Gonçalves Geracino 
- Marcos Vinícius Piaia 
- Tatiany Fermino Rodrigues de Oliveira

## Ambiente

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/Equipe6-2020/Trabalho-INF325/master)

### Este repositório
Estrutura de pastas do repositório:
~~~
├── README.md
├── binder -> dados para a construção de ambiente
│   ├── apt.txt
│   └── requirements.txt
├── dataset
│   ├── json_with_relations.json
│   ├── olist_customers_dataset.json
│   ├── olist_geolocation_dataset.json
│   ├── olist_order_items_dataset.json
│   ├── olist_order_payments_dataset.json
│   ├── olist_orders_dataset.json
│   ├── olist_products_dataset.json
│   ├── olist_sellers_dataset.json
│   └── product_category_name_translation.json
├── notebooks
│   ├── 0.\ README.ipynb
│   ├── 1.\ MongoSetup.ipynb
│   ├── 2.\ Neo4jSetup.ipynb
│   ├── 3.\ MongoEnv.ipynb
│   └── 4.\ Neo4jEnv.ipynb
└── resources
    ├── Neo4JPayloads
    │   ├── Neo4JPayloads
    │   │   ├── Business
    │   │   │   └── ProcessInformationBusiness.cs
    │   │   ├── Models
    │   │   │   ├── Category.cs
    │   │   │   ├── Customer.cs
    │   │   │   ├── ModelBase.cs
    │   │   │   ├── Order.cs
    │   │   │   ├── OrderItem.cs
    │   │   │   ├── Product.cs
    │   │   │   └── Region.cs
    │   │   ├── Neo4JPayloads.csproj
    │   │   ├── Program.cs
    │   │   ├── Querys
    │   │   │   ├── Query.cs
    │   │   │   └── QueryBase.cs
    │   │   ├── bin
    │   │   │   └── Debug
    │   │   │       └── netcoreapp3.1
    │   │   │           ├── Neo4JPayloads.deps.json
    │   │   │           ├── Neo4JPayloads.dll
    │   │   │           ├── Neo4JPayloads.exe
    │   │   │           ├── Neo4JPayloads.pdb
    │   │   │           ├── Neo4JPayloads.runtimeconfig.dev.json
    │   │   │           ├── Neo4JPayloads.runtimeconfig.json
    │   │   │           └── Newtonsoft.Json.dll
    │   │   └── obj
    │   │       ├── Debug
    │   │       │   └── netcoreapp3.1
    │   │       │       ├── Neo4JPayloads.AssemblyInfo.cs
    │   │       │       ├── Neo4JPayloads.AssemblyInfoInputs.cache
    │   │       │       ├── Neo4JPayloads.assets.cache
    │   │       │       ├── Neo4JPayloads.csproj.CopyComplete
    │   │       │       ├── Neo4JPayloads.csproj.CoreCompileInputs.cache
    │   │       │       ├── Neo4JPayloads.csproj.FileListAbsolute.txt
    │   │       │       ├── Neo4JPayloads.csprojAssemblyReference.cache
    │   │       │       ├── Neo4JPayloads.dll
    │   │       │       ├── Neo4JPayloads.exe
    │   │       │       ├── Neo4JPayloads.genruntimeconfig.cache
    │   │       │       └── Neo4JPayloads.pdb
    │   │       ├── Neo4JPayloads.csproj.nuget.dgspec.json
    │   │       ├── Neo4JPayloads.csproj.nuget.g.props
    │   │       ├── Neo4JPayloads.csproj.nuget.g.targets
    │   │       ├── project.assets.json
    │   │       └── project.nuget.cache
    │   └── Neo4JPayloads.sln
    └── Neo4JQueries
        └── Neo4JQueries.md
~~~
