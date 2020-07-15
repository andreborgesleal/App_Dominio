# App_Dominio
Framework tipo Class Library orquestrador da arquitetura dos sistemas

**Component**: Este componente contém os objetos utilizados para envio de e-mails, os modelos de objetos usados nas listagens dos grids e a superclasse “Repository” que dela derivam todos os demais objetos da aplicação que serão usados para apresentação dos dados (Viewmodels). 

**Contratos**: Este componente contém todas as interfaces que regem a padronização das principais funcionalidades do sistema, tais como:  

- Exibição de conteúdo em combobox (dropdownlist); 
- CRUDS (inclusão, alteração, exclusão e consulta) de tabelas e cadastros; 
- Listagem de dados nos grids da tela; 
- Controle de paginação nos grids da tela; 
- Retorno e controle de mensagens de validação 
- Controle de segurança (autenticação de usuário, menus do sistema, etc) 

**Controllers**: Este componente contém os objetos responsáveis pela camada de controle da aplicação. Ele implementa os métodos em alto nível (Superclasse) responsáveis pelo controle das operações de exibição de mensagens, operações de inclusão, alteração, exclusão, consulta, operações de listagem entre outras. Todas os objetos controllers das aplicações herdam das superclasses deste Controller.

**Entidades**: Este componente possui as classes que modelam as entidades do banco de dados. Seus atributos correspondem de forma idêntica aos atributos das tabelas no banco de dados. 

**Repositories**: Este componente contém os objetos usados pelo Sistema de Segurança  

**Security**: Este componente implementa as classes responsáveis pela comunicação entre as demais camadas deste componente com o Sistema. Implementa também as operações de segurança para autenticação do usuário, esqueci minha senha e alteração de senha.  
 
# Armazenamento e Recuperação de dados 

SQL Server

A persistência e recuperação de dados e o controle operacional serão feitas através de instruções LINQ que mapearão diretamente as tabelas do banco de dados ou nos casos mais essenciais, através de stored procedures. Essas stored procedures estarão assinadas no objeto “App_DominioContext” constante na camada “Entidades” do componente “App_Dominio” e serão chamadas nas classes de negócio constantes na camada “Models” da aplicação. 

A aplicação realiza conexão em duas bases de dados: banco de dados do sistema e banco de dados de Segurança. 

A conexão é feita através de dois componentes, respectivamente: *ApplicationContext* e *SecurityContext*

# Auditoria e Tratamento de Erros 

As exceções serão tratadas por camada, isto é, se uma exceção for lançada na camada de serviço, a mesma deve ser primeiramente tratada nesta camada antes de ser lançada para uma camada superior. 

Na aplicação web haverá uma camada macro de tratamento de erro no qual um bloco de código possivelmente não tratado pelo desenvolvedor será capturado pela aplicação e logado para posterior avaliação. 

Existirá uma classe chamada “App_DominioException” constante no componente “App_Dominio” que vai herdar da classe “Exception”, nativa do framework. Esta classe será responsável pelo tratamento da manipulação dos erros nas 3 camadas da aplicação. Ela se encarregará de atribuir as mensagens amigáveis ao usuário e registrar em arquivo XML dinâmico (um arquivo diário contendo sequencialmente as ocorrências), o trace com a descrição resumida e detalhada do correspondente erro. A consulta a esses arquivos poderá ser tratada e disponibilizada a partir de uma funcionalidade específica de consulta de erros.  

Esta funcionalidade estará cadastrada no sistema de segurança e conterá os links para os arquivos diários de logs. 

O LOG de auditoria na camada web ou transações de sistema via comandos LINQ ou stored procedures serão armazenados em banco de dados. O registro deste LOG será identificado por um número único, sequencial, que não poderá ser excluído por questões de segurança e guardará o LOGIN do usuário, data e hora que a funcionalidade foi executada, identificação da funcionalidade, a ação realizada que pode ser de inclusão, alteração, consulta ou exclusão e uma descrição resumida/detalhada da ocorrência. Também será registrado o namespace do processo. 

O registro ocorrerá de acordo com a relevância do caso de uso, ou seja, processos simples como cadastros básicos possuirão uma ocorrência no log de auditoria, todavia processos mais complexos, que envolvam cálculos ou atualizações em várias tabelas terão mais de um registro auditado para a mesma funcionalidade. Nestes casos de alta relevância, além de registrar os atributos citados acima, também serão registrados os atributos e valores antes e após a execução da operação. 

Será possível também, dentro de um mesmo processo, registrar vários logs como se fosse uma espécie de trilha percorrida, mantendo assim um histórico mais minucioso e detalhado das operações realizadas, criando assim pontos de impacto para melhor depuração de possíveis falhas que possam vir a ocorrer. 

Assim como o log de erros, o log de auditoria também possuirá uma funcionalidade cadastrada no sistema de segurança, cujo objetivo é exibir a auditoria de acordo com os parâmetros disponibilizados ao usuário. 

Já nos processos Batch, o LOG de auditoria e erros será realizado por JOB e armazenado em arquivo texto diário. O LOG será tratado a partir da camada de controle e vai registrar o trace das operações realizadas pelo usuário em cada camada, similar a uma memória de cálculo, registrando o passo a passo das ações executadas no sistema. 

Exemplo: 

<?xml version="1.0" encoding="utf-8"?> 

<LOG> 

  <LOG ID="1086159964" Data="14/03/2014 11:24:19"> 

    <Sessao>nbro2hegv3u1hkvptls0nc5n</Sessao> 

    <IP>::1</IP> 

    <HostName>ANDRE-VAIO\André</HostName> 

    <Mensagem>O índice estava fora dos limites da matriz.</Mensagem> 

    <NameSpace>Sindemed.Models.Report.RelacaoGeralReport =&gt; report01/listparam</NameSpace> 

    <Context /> 

    <StackTrace>   em Sindemed.Models.Report.RelacaoGeralReport.Bind(Nullable`1 index, Int32       pageSize, Object[] param) na c:\Users\André\Source\Repos\SINDMED\Sindemed\Models\Report\RelacaoGeralReport.cs:linha 31 

   em App_Dominio.Component.ReportRepository`2.ListRepository(Nullable`1 index, Int32 pageSize, Object[] param) na c:\Users\André\Source\Repos\App_Dominio\App_Dominio\App_Dominio\Component\ListViewRepository.cs:linha 97 

   em App_Dominio.Component.ListViewRepository`2.getPagedList(Nullable`1 index, Int32 pageSize, Object[] param) na c:\Users\André\Source\Repos\App_Dominio\App_Dominio\App_Dominio\Component\ListViewRepository.cs:linha 30</StackTrace> 

</LOG> 

# Segurança 

Todas as funcionalidades do sistema estão cadastradas no banco de dados do sistema de segurança. Cada funcionalidade está vinculada a um ou muitos grupos de usuários. Somente o usuário que pertencer ao grupo poderá visualizar e acessar a respectiva funcionalidade. 

Quando o usuário efetua o seu LOGIN, a aplicação verifica a qual grupo ele pertence e recupera todas as funcionalidades do respectivo grupo para exibir o menu de opções do site. 

**Tempo de Sessão**

Existe um relógio interno que cronometra o tempo que o usuário permanece estático em uma dada página. Sempre que o usuário muda de tela, o sistema checa esse tempo e caso tenha atingido um limite de 15 minutos (esse tempo é parametrizado), o sistema encerra a sessão do usuário e redireciona para a tela de LOGIN para que ele faça uma nova autenticação. 

**Acesso sem autenticação**

Se algum usuário não credenciado tentar acessar qualquer formulário do sistema diretamente pela URL sem proceder a autenticação, o sistema também irá redirecioná-lo para a tela de LOGIN.  

Acesso a uma página não autorizada, mesmo estando autenticado: 

Outro cenário que pode ocorrer é o usuário estar autenticado e tentar acessar uma página que não está autorizada para o grupo a que pertence.  

Por exemplo, um determinado cliente, uma vez “logado”, pode tentar acessar o cadastro de outro cliente ou tentar acessar o relatório com a relação geral de todos os clientes.  

Esta funcionalidade só é disponibilizada para quem pertencer ao grupo “Administração”.  

O grupo “Cliente” não tem acesso a esse recurso. Porém, se o cliente tentar fazer o acesso, o sistema irá apresentar uma mensagem informando que ele não está autorizado a executar tal operação. 

# Sistema de Segurança 

O Sistema de Segurança faz parte do contexto do projeto. É um recurso apartado e tem como objetivo administrar as contas de usuários que farão acesso ao Sistema e também ao Sistema Financeiro/Contábil. 

Todas as funcionalidades de cada sistema ficarão gravadas em banco de dados. Haverá um ou mais de um usuário administrador que terá acesso ao Sistema de Segurança para executar a gestão das contas dos usuários. 

Os usuários estarão sempre vinculados a um grupo. Cada grupo terá suas funcionalidades específicas. Os usuários só conseguirão visualizar e acessar as funcionalidades atribuídas ao seu grupo. 

No Sistema de segurança também é possível consultar o Log de Auditoria de todas as operações realizadas no sistema. 
