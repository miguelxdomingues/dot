
# 2019 - Setup da Framework 

Este documento descreve os passos necessários para configurar o ambiente de desenvolvimento e fazer debug a um produto construído com a Elevation Framework.

**Última atualização**  
Projeto Iodine, Dezembro 2019

## Source Code

**Elevation Framework**

Framework  
`$/Elevation3/FW/4.00/Development-Core`  

Módulos da Framework  
`$/Elevation3/FW/4.00/Development-Core/Framework/Development/Server/Modules`

**Primavera.JasminPremium (produto ou protótipo)**

Produto
`$/Elevation3/FW/Prototyping/Development-Core`  

Módulos do Produto
`$/Elevation3/FW/Prototyping/Development-Core/Modules`

## Passos para executar locamente o host do produto
 
**Parte do Servidor**

1. GLV da linha com os módulos da framework  
    `$/Elevation3/FW/4.00/Development-Core`

2. GLV da linha com os módulos de produto ou protótipo  
    `$/Elevation3/FW/Prototyping/Development-Core`   
   
3. Abrir a solução do produto   
    `$/Elevation3/FW/Prototyping/Development-Core/Products/Primavera.JasminPremium/Primavera.JasminPremium.sln`

4. Para os passos seguintes considerar o seguinte caminho base
    `$/Elevation3/FW/Prototyping/Development-Core/Products/Primavera.JasminPremium`

5. Restore dos nuget packages

6. Rebuild da solução

7. Criar a base de dados do produto e configurar a connection string 

   - Como este passo depende da build de BD, se houver nova build é necessário fazer GLV, Restore, Transform

   - Configurar a connection string para o SQL Server local
    
     - Unit tests `/Deployment/Settings/Provisioning.json`

        ```json
        // Substituir no grupo das LOCAL
        "LocalDatabaseServer": "MDOMINGUES2\\SQL2017DEV",
        ```   

      - Run-time `\Host\App_Data\Subscriptions.json`

        ```json
        // Substituir nas subscrições que vamos usar:
        // "SubscriptionKey": "LocalPrototypeDefaultPT",
        // "SubscriptionKey": "LocalPrototypeTestPT",        
        "value": "Data Source=MDOMINGUES2\\SQL2017DEV; ..."
        ```   

   - Transformar o tt, que usa o json anterior, para gerar as scripts de BD  
    `/Deployment/GeneratedCode/Provisioning.tt`

   - Executar os testes unitários para criar as BDs desejadas   
     (build para o `Test Explorer` descobrir os testes)
   `Execute_Provisioning_PrototypeDevLocalDefaultPT`   
   `Execute_Provisioning_PrototypeDevLocalTestPT`   

   - Outros ficheiros relacionados 
     - Resources DB `\Database\App.config`

8. Em alguns cenários (p. ex. caso nunca o tenham feito) pode ser preciso compilar a parte cliente
   - Seguir os passos da secção [Cliente] e depois voltar aqui

9. F5 no `Host` da solução e temos o produto a correr localmente!

**Parte do Cliente**

1.  Abrir a ClientApp `/Host/ClientApp`   

2. Executar o comando `npm install` para instalar os packages

   - Se for a primeira vez que executas este comando poderá ser necessária alguma configuração adicional (permissões, etc), vê os erros que podem ocorrer nos passos a seguir.

   - Se der o erro `Unable to authenticate, your authentication token seems to be invalid` (fazer apenas 1x ou quando expirar)

      - Abrir uma linha de comandos em modo administrador e configurar o NPM, tal como está documentado aqui:  
      https://devops.primaverabss.com/elevation-docs/developercookbook/developercookbook.configuracaonpm/#configuracao-npm

          ```
          npm install -g npmrc
          npmrc
          npmrc -c primavera
          npmrc primavera
          npmrc
          ```
      - Depois do último comando verifica que selecionou o primavera como default (tem um *)

      - Editar o ficheiro criado no passo anterior `%USERPROFILE%\.npmrcs\primavera`

        - Obter as credenciais de acesso aos feeds npm do nosso projeto (usar um dos links abaixo):

          https://tfs.primaverabss.com/tfs/P.TEC.Elevation/Elevation3/_packaging?feed=elevation4-dev&_a=feed
          https://tfs.primaverabss.com/tfs/P.TEC.Elevation/Elevation3/INT-FW/_packaging?feed=elevation4&_a=feed

         - Selecionar o feed `elevation4-dev`, clicar em `Connect to feed`, selecionar `\npm`, clicar em `Generate npm credentials`
           - Copiar e colar a parte do `registry=` para o nosso ficheiro npmrcs
           - Copiar e colar a parte das credentials para o nosso ficheiro npmrcs

         - Selecionar o feed `prototype`, clicar em `Connect to feed`, e depois selecionar `\npm`
           - Copiar e colar (igual ao anterior)
     
   - Se der erro do Git, (re)instalar o Git para windows 
     - download https://git-scm.com/download/win   
     - testar na powershell com `git --version`

   - Se der erro dos módulos, apagar o `package.lock.json` e o `node_modules`

   - Se der erro dos módulos, abrir o `package.json` e apagar os módulos que não existem (p.ex.)
     ```json
        "@prototype/businesscore": "prototype-mainline",
        "@prototype/financialcore": "prototype-mainline",
        "@prototype/jasmincomponents": "^2.0.1-6",
        "@prototype/sales": "prototype-mainline",
        "@prototype/shipping": "prototype-mainline",
     ```

3. Executar o comando `npm run build` para compilar a parte de cliente

   - Se for a primeira vez que executas este comando poderá ser necessária alguma configuração adicional (permissões, etc), vê os erros que podem ocorrer nos passos a seguir.

   - Se der erro, tentar em debug, às vezes funciona com o `npm run debug`, e depois é ver as diferenças das scripts no ficheiro `package.json`

   - Se der erro na escrita em ficheiros tirar os atributos `read-only`
   
   - se der erro `EPERM: operation not permitted, open '\ClientApp\src\assets\i18n\en-us\clientApp.components.lang.json` ir às propriedades da pasta `\ClientApp\src\assets` e retirar o `Read-only`

   - Se der erro porque não encontra determinado ficheiro, e caso estejamos a usar um `symbolic link` no caminho do windows, é necessário adicionar ao `package.json` o switch `--preserve-symlinks` (google for `Build fails when using symbolic link folder in Windows`)
     ```
     "scripts": {
       "build": "ng build --prod --output-hashing none --aot false --build-optimizer false --preserve-symlinks",
     },
     ```

   - Se der erro, find no projeto por `prototype\*` e/ou `@prototype/*`:
     - apagar módulos e `jasmincomponents`

   - Se der erro de `clean:*` apagar a pasta `ClientApp\dist` e voltar a repetir cmd

   - Se der erro nos `gulp` apagar a pasta `ClientApp\node_modules\gulp*` e voltar a fazer o `npm install`

   - Se der erro apagar `node_modules` e `package-lock.json` e repetir cmd

   - Se der erro, corrigir as rotas apagando os módulos desnecessários em  
   `\ClientApp\src\app\app.routes.ts`  
   `\ClientApp\src\app\app.routes.tt`  
 
   - se der erro, apagar o `prototype` em `\ClientApp\src\app\app.module.ts`

4. Executar o comando `npm run debug` para fazer debug à parte de cliente

## Passos para fazer debug ao módulo ou testar no produto as alterações ao módulo:

1. Abrir a solução do módulo:  

    - de framework  
    `$/Elevation3/FW/4.00/Development-Core/Framework/Development/Server/Modules/CorePatterns/Primavera.Core.Patterns.sln`  

    - ou produto  
    `$/Elevation3/FW/Prototyping/Development-Core/Modules/Billing/Primavera.Billing.sln`  

2. Alterar o que for necessário no código do módulo (modelar ou código custom)

3. Compilar o módulo (transform and rebuild)

4. Ainda a partir da solução do módulo, substituir os packages usados pela solução do produto

   - Ir ao menu `ELEVATION \ NuGet \ Replace Assemblies in Local Repository`

   - Escolher da lista de versões a versão do package que queremos substituir, tipicamente será a que estiver a ser usada pelo produto

   - Selecionar a pasta destino `C:\PRJNET\Elevation3\FW\Prototyping\Development-Core\_packages`

5. A partir da solução do produto fazer debug ao módulo que foi compilado localmente

   - IMPORTANTE! Não fazer restore dos packages, caso contrário iremos buscar novamente o package e deixamos de usar o que tem as nossas alterações

   - Abrir na solução do produto o ficheiro do módulo onde queremos colocar o breakpoint, sendo que antes copiamos o full path do ficheiro a partir da solução do módulo

   - Depois é só colocar o breakpoint nos locais de debug; é boa prática fazer sempre Clean and Rebuild

   - Caso o breakpoint não seja detetado (bolinha branca e não vermelha) as causas podem ser várias, entre elas:

     - Substituimos a versão errada do package nuget

     - Será necessário fazer Clean and Rebuild na solução do produto para que as alterações sejam detetadas

     - Em último caso, usar o Clean do SDK no menu `ELEVATION | Solution | Clean`, para limpar a pasta `bin` e todo o lixo que possa existir
 