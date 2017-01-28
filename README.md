## Aplicativo de Lembretes

	Aplicativo para cadastro e listagem de lembretes. 
	Cada lembrete é composto de título, descrição, data limite e status concluído.

## Objetivo técnico

	Objetivo do aplicativo é demostrar o conhecimento em construção de projetos web.
	
## Características técnicas 
	
### Backend | Asp.Net Core 

	**Reminders.Domain e Reminders.Data**
	- São utilizados patterns MVVM e Repository.
	- Foi utilizado o mapeamento de objetos relacionais Microsoft Entity Framework.
	- Para persistência de dados é utilizado o banco de dados SQL Server.

### Frontend | Asp.Net Core MVC [Acesse o App](http://reminderscoreapp.azurewebsites.net/)
  
	**Reminders.App**
	- Neste aplicativo é utilizado a estrutura MVC da plataforma Asp.Net Core.
	- Aplicativo responsivo utilizando Bootstrap Framework.

### Frontend | Asp.Net Core Web Api [Acesse a Web Api](http://reminderscoreapi.azurewebsites.net/swagger/ui/)

	**Reminders.Api**
	- Neste aplicativo é utilizado a estrutura MVC da plataforma Asp.Net Core com as configurações Web.Api.

### Frontend | AngularJs [Acesse o App com AngularJs](http://remindersangular.azurewebsites.net/)
	
	**Reminders.AngularJs**
	- Neste aplicativo é utilizado a framework AngularJs.
	- Aplicativo responsivo utilizando Bootstrap Framework.
	
## Como configurar

	- Será necessário ter uma instancia do SQL Server disponível para utilização. 
	- O usuário do SQL Server deverá ter permissão para criar base de dados, pois o aplicativo criara a base assim que a aplicação compilar pela primeira vez. 
	- A string de conexão devera ser colocada no arquivo "appsettings.json" na chave "StringConnectionReminders" do projeto "Reminders.App".