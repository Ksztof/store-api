# Perfume-shop

Online shop

# Run

1. Download project/git pull main
2. set default project as Domain,
3. start migration with `update-database` command typed in nuget console
4. In database run query:
   SET IDENTITY_INSERT dbo.ProductCategories ON;
   insert into dbo.ProductCategories(Id, Name) values (1, 'catA');
   insert into dbo.ProductCategories(Id, Name) values (2, 'catB');
   insert into dbo.ProductCategories(Id, Name) values (3, 'catC');

# Review

- Wszystkie komentarze bêd¹ jako `//KM ...`
- Z takich na pierwszy rzut oka masz jakieœ dziwne formatowanie wciêæ, ale to drobnica
- mo¿esz sobie bia³e znaki wyœwietliæ jako Edit > Advanced > View White Space (https://stackoverflow.com/questions/6255839/show-space-tab-crlf-characters-in-editor-of-visual-studio)
- jak chcesz szybko sformatowaæ dokument to Edit > Advanced > Format Document
- Ogólnie projekt nie ma struktury DDD, która powinna byæ zorientowana na zachowania i funkcjonalnoœci, a bardziej ma strukturê "Component Based"
	- Zauwa¿, ¿e podzia³ w Twoim projekcie to Repositories, Services, Validators, a bardziej oczekiwany jest podzia³ na Orders, Products itd.
- Uwa¿am, ¿e klasa `ValidationService` nie robi nic po¿ytecznego, zauwa¿, ¿e dodanie ka¿dego nowego Validatora spowoduje edycjê tej klasy, dodatkowo nikt mi nie broni wywo³aæ `IValidationService.ValidateCreateProductForm` w `CartService`.
- Zapoznaj siê proszê z ide¹ "Clean Architecture" ona fajnie wspó³gra z DDD, polecam - https://www.youtube.com/watch?v=fe4iuaoxGbA&ab_channel=MilanJovanovi%C4%87. Chodzi mi g³ównie o projekty: Domain, Infrastructure, Application, API.
- Projekt `Core` jest ok jeœli zawiera rzeczy wspólne dla wszystkiego, ale nie odpowiedzialnoœci biznesowe
- Zauwa¿, ¿e Twoja klasa `CartService` jest bardzo stanowa, ca³a logika jest zale¿na od tego czy w cookies masz CartId. Rozwa¿y³bym przeniesienie logiki zarzadzania Id koszyka - `IGuestSessionService` do kontrolera i rozszerzenie `CartService.AddProductToCartAsync(cartId, productId, productQuantity.Quantity);` o `CartId`
- Zamiast `OrderResponse`, `CartResponse` itd. rzuæ okiem na https://www.youtube.com/watch?v=WCCkEe_Hy2Y&ab_channel=MilanJovanovi%C4%87. Jest coœ takiego jak Result Pattern, myœlê, ¿e mo¿e Ci siê spodobaæ
- Jeœli chodzi o wszelkiego rodzaju `*Form` to one zdecydowanie nie powinny byæ w projekcie Domain. Jeœli chcesz przekazywaæ ten sam obiekt z kontrolera do serwisu (co teraz robisz) to wszystkie formy powinny siê znaleŸæ w projekcie Core, dok³adnie tam gdzie jest serwis.
- WA¯NE: Twój projekt Domain powinien zawieraæ tylko i wy³¹cznie logikê biznesow¹ bez ¿adnych zale¿noœci. Nie mo¿e byæ tak, ¿e w Domain masz requesty wejœciowe, które s¹ u¿ywane przez kontroler
- Osobiœcie nie jestem fanem u¿ywania tego samoego w kontrolerze i serwisie jako wejœcia np. `CreateProductForm`
- Patrz¹c na projekt wydaje mi siê, ¿e nale¿a³oby go sformatowaæ po "funkcjonalnoœciach" i tak porozdzielaæ po folderach:
	- Orders
	- Products
	- Carts (GuestSessionService, CartService)
	- Authentication (TokenService, UserService)
	- Notification (EmailService, EmailSender)
- To samo zrobiæ z repozytoriami.
- ...
