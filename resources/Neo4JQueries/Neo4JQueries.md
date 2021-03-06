# Loading dos dados para o banco Neo4J

- DELETE ALL nodes
```cypher
MATCH (n)
DETACH DELETE n
```

- Carregamento dos dados de cliente
```cypher
CALL apoc.load.json("dataset/olist_customers_dataset.json") yield value
UNWIND value as customer
WITH (customer)
WHERE customer.customer_city in ["campinas"]
MERGE (cliente:Cliente {id:customer.customer_id}) ON CREATE SET cliente.nome = customer.customer_id
MERGE (regiao:Regiao {id:customer.customer_zip_code_prefix}) ON CREATE SET regiao.nome = customer.customer_zip_code_prefix
MERGE (cidade: Cidade {id:customer.customer_city}) ON CREATE SET cidade.nome = customer.customer_city
MERGE (estado: Estado {id:customer.customer_state}) ON CREATE SET estado.nome = customer.customer_estado
MERGE (cliente)-[:RESIDE_EM]->(regiao)
MERGE (regiao)-[:FAZ_PARTE_DE]->(cidade)
MERGE (cidade)-[:DENTRO_DE]->(estado)
````

- Carregamento dos pedidos
```cypher
CALL apoc.load.json("dataset/olist_orders_dataset.json") yield value
UNWIND value as orderList
WITH (orderList)
MATCH (cliente:Cliente)
WHERE orderList.customer_id = cliente.id
MERGE (pedido:Pedido {id : orderList.order_id}) ON CREATE SET pedido.nome = orderList.order_id
MERGE (cliente)-[:REALIZOU]->(pedido)
````

- Carregamento da lista de itens de um pedido
```cypher
CALL apoc.load.json("dataset/olist_order_items_dataset.json") yield value
UNWIND value as orderListItems
WITH (orderListItems)
MATCH (pedido:Pedido)
WHERE orderListItems.order_id = pedido.id
MERGE (itempedido:itempedido {id : orderListItems.order_id + '-' + orderListItems.order_item_id}) 
    ON CREATE SET itempedido.nome = orderListItems.order_id + '-' + orderListItems.order_item_id,
                  itempedido.preco = orderListItems.price,
                  itempedido.produto = orderListItems.product_id,
                  itempedido.fornecedor = orderListItems.seller_id
MERGE (produto:Produto {id: orderListItems.product_id}) 
    ON CREATE SET produto.nome = orderListItems.product_id
MERGE (fornecedor:Fornecedor {id: orderListItems.seller_id})
    ON CREATE SET fornecedor.nome = orderListItems.seller_id
MERGE (itempedido)-[:PERTENCE]->(pedido)
MERGE (produto)-[:ESTA_EM {iditem : orderListItems.ordr_item_id}]->(pedido)
MERGE (produto)-[:EH_VENDIDO]->(fornecedor)
WITH (pedido)
MATCH (cli)-[:REALIZOU]->(ped)
WHERE ped.id = pedido.id
MERGE (produto)-[:FOI_COMPRADO]->(cli)
```

- Construção do database utilizando script único
```cypher
CALL apoc.load.json('dataset/json_with_relations.json') yield value
UNWIND value.OrderItems as items
MERGE (pedido:Pedido {id : value.order_id})
    ON CREATE SET pedido.nome = value.order_id,
                  pedido.status = value.order_status,
                  pedido.order_purchase_timestamp = value.order_purchase_timestamp,
                  pedido.order_approved_at = value.order_approved_at,
                  pedido.order_delivered_carrier_date = value.order_delivered_carrier_date,
                  pedido.order_delivered_customer_date = value.order_delivered_customer_date,
                  pedido.order_estimated_delivery_date = value.order_estimated_delivery_date
MERGE (cliente:Cliente {id : value.customer_id})
    ON CREATE SET cliente.nome = value.customer_id
MERGE (regiao : Regiao {id : value.Customer.customer_zip_code_prefix})
    ON CREATE SET regiao.nome = value.Customer.customer_zip_code_prefix
MERGE (cidade : Cidade {id : value.Customer.customer_city})
    ON CREATE SET cidade.nome = value.Customer.customer_city
MERGE (estado : Estado {id : value.Customer.customer_state})
    ON CREATE SET estado.nome = value.Customer.customer_state
MERGE (item_pedido: ItemPedido {id: value.order_id+'-'+items.order_item_id})
    ON CREATE SET item_pedido.nome = value.order_id+'-'+items.order_item_id,
                  item_pedido.preco = items.price,
                  item_pedido.shipping_limit_date = items.shipping_limit_date,
                  item_pedido.frete = items.freight_value
MERGE (produto: Produto {id : items.product_id})
    ON CREATE SET produto.nome = items.product_id,
                  produto.product_name_lenght = items.product.product_name_lenght,
                  produto.product_description_lenght = items.product.product_description_lenght,
                  produto.product_photos_qty = items.product.product_photos_qty,
                  produto.product_weight_g = items.product.product_weight_g,
                  produto.product_length_cm = items.product.product_length_cm,
                  produto.product_height_cm = items.product.product_height_cm,
                  produto.product_width_cm = items.product.product_width_cm
MERGE (fornecedor : Fornecedor {id : items.seller_id})
    ON CREATE SET fornecedor.nome = items.seller_id
MERGE (categoria : Categoria {id : items.product.product_category_name})
    ON CREATE SET categoria.nome = items.product.product_category_name
MERGE (regiaoFornecedor : Regiao {id : COALESCE(items.seller.seller_zip_code_prefix, "UNKNOWN")})
    ON CREATE SET regiaoFornecedor.nome = COALESCE(items.seller.seller_zip_code_prefix, "UNKNOWN")
MERGE (cidadeFornecedor : Cidade {id : COALESCE(items.seller.seller_city, "UNKNOWN")})
    ON CREATE SET cidadeFornecedor.nome = COALESCE(items.seller.seller_city, "UNKNOWN")
MERGE (cliente)-[:COMPROU {id_pedido : value.order_id}]->(produto)
MERGE (cliente)-[:MORA]->(regiao)
MERGE (regiao)-[:EH_PARTE_DE]->(cidade)
MERGE (cidade)-[:ESTA_LOCALIZADA]->(estado)
MERGE (produto)-[:EH_DE]->(item_pedido)
MERGE (produto)-[:EH_PRODUZIDO]->(fornecedor)
MERGE (produto)-[:EH_MONTADO]->(regiaoFornecedor)
MERGE (fornecedor)-[:PERTENCE]->(regiaoFornecedor)
MERGE (regiaoFornecedor)-[:EH_PARTE_DE]->(cidadeFornecedor)
MERGE (produto)-[:FOI_COMPRADO_POR]->(cliente)
MERGE (produto)-[:POSSUI]->(categoria)
MERGE (fornecedor)-[:FORNECE_CATEGORIA]->(categoria)
````

# Consultas

- produtos mais vendidos de uma cidade
```cypher
MATCH (cidade:Cidade)
WHERE cidade.id = "sao paulo"
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[:FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
WITH pro.id AS product, COUNT(cliAux) as qtdeProduto
RETURN product, qtdeProduto
ORDER BY qtdeProduto DESC
```

- categorias mais vendidas em uma cidade
```cypher
MATCH (cidade:Cidade)
WHERE cidade.id = "sao paulo"
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[:FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
MATCH (proAux)-[:POSSUI]->(cat)
WHERE proAux.id = pro.id
WITH cat.id AS categoria, COUNT(pro) as qtdeProduto
RETURN categoria, qtdeProduto
ORDER BY qtdeProduto DESC
```

- cidades que venderam mais produtos
```cypher
MATCH (cidade:Cidade)
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[r:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
WITH cid.id as city, COUNT(pro.id) as qtdeProduto
RETURN city, qtdeProduto
ORDER BY qtdeProduto DESC
```

- Cidades que apresentaram o maior volume de vendas
```cypher
MATCH (cidade:Cidade)
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[:FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
MATCH (proItem)-[:EH_DE]->(item)
WHERE proItem.id = pro.id
WITH cidade.id as city, SUM(item.preco) as valorVendas
RETURN city, ROUND(valorVendas*100)/100 as vendas
ORDER BY valorVendas DESC
```

- Volume de vendas das categorias de produtos em uma determinada cidade
```cypher
MATCH (cidade:Cidade)
WHERE cidade.id = "sao paulo"
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[:FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
MATCH (proAux)-[:POSSUI]->(cat)
WHERE proAux.id = pro.id
MATCH (proItem)-[:EH_DE]->(item)
WHERE proItem.id = proAux.id
WITH cidade.id as city, cat.id AS categoria, SUM(item.preco) as valorVendas
RETURN city, categoria, valorVendas
ORDER BY valorVendas DESC
```

- Cidades que apresentaram o maior valor medio de frete cobrado em cada venda
```cypher
MATCH (cidade:Cidade)
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[:FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
MATCH (proItem)-[:EH_DE]->(item)
WHERE proItem.id = pro.id
WITH cidade.id as city, AVG(item.frete) as valorFrete
RETURN city, ROUND(valorFrete*100)/100 as Frete
ORDER BY valorFrete DESC
```

- Valor medio de frete com informação de categoria de produto em uma determinada cidade
```cypher
MATCH (cidade:Cidade)
WHERE cidade.id = "sao paulo"
MATCH (reg)-[:EH_PARTE_DE]->(cid)
WHERE cid.id = cidade.id
MATCH (cli)-[:MORA]->(regAux)
WHERE regAux.id = reg.id
MATCH (pro)-[:FOI_COMPRADO_POR]->(cliAux)
WHERE cliAux.id = cli.id
MATCH (proAux)-[:POSSUI]->(cat)
WHERE proAux.id = pro.id
MATCH (proItem)-[:EH_DE]->(item)
WHERE proItem.id = proAux.id
WITH cidade.id as city, cat.id AS categoria, AVG(item.frete) as valorFrete
RETURN city, categoria, ROUND(valorFrete*100)/100 as Frete
ORDER BY valorFrete DESC
```
