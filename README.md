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

- Wszystkie komentarze b�d� jako `//KM ...`
- Z takich na pierwszy rzut oka masz jakie� dziwne formatowanie wci��, ale to drobnica
- mo�esz sobie bia�e znaki wy�wietli� jako Edit > Advanced > View White Space (https://stackoverflow.com/questions/6255839/show-space-tab-crlf-characters-in-editor-of-visual-studio)
- jak chcesz szybko sformatowa� dokument to Edit > Advanced > Format Document
- Og�lnie projekt nie ma struktury DDD, kt�ra powinna by� zorientowana na zachowania i funkcjonalno�ci, a bardziej ma struktur� "Component Based"
	- Zauwa�, �e podzia� w Twoim projekcie to Repositories, Services, Validators, a bardziej oczekiwany jest podzia� na Orders, Products itd.
- Uwa�am, �e klasa `ValidationService` nie robi nic po�ytecznego, zauwa�, �e dodanie ka�dego nowego Validatora spowoduje edycj� tej klasy, dodatkowo nikt mi nie broni wywo�a� `IValidationService.ValidateCreateProductForm` w `CartService`.
- Zapoznaj si� prosz� z ide� "Clean Architecture" ona fajnie wsp�gra z DDD, polecam - https://www.youtube.com/watch?v=fe4iuaoxGbA&ab_channel=MilanJovanovi%C4%87. Chodzi mi g��wnie o projekty: Domain, Infrastructure, Application, API.
- Projekt `Core` jest ok je�li zawiera rzeczy wsp�lne dla wszystkiego, ale nie odpowiedzialno�ci biznesowe
- Zauwa�, �e Twoja klasa `CartService` jest bardzo stanowa, ca�a logika jest zale�na od tego czy w cookies masz CartId. Rozwa�y�bym przeniesienie logiki zarzadzania Id koszyka - `IGuestSessionService` do kontrolera i rozszerzenie `CartService.AddProductToCartAsync(cartId, productId, productQuantity.Quantity);` o `CartId`
- Zamiast `OrderResponse`, `CartResponse` itd. rzu� okiem na https://www.youtube.com/watch?v=WCCkEe_Hy2Y&ab_channel=MilanJovanovi%C4%87. Jest co� takiego jak Result Pattern, my�l�, �e mo�e Ci si� spodoba�
- Je�li chodzi o wszelkiego rodzaju `*Form` to one zdecydowanie nie powinny by� w projekcie Domain. Je�li chcesz przekazywa� ten sam obiekt z kontrolera do serwisu (co teraz robisz) to wszystkie formy powinny si� znale�� w projekcie Core, dok�adnie tam gdzie jest serwis.
- WA�NE: Tw�j projekt Domain powinien zawiera� tylko i wy��cznie logik� biznesow� bez �adnych zale�no�ci. Nie mo�e by� tak, �e w Domain masz requesty wej�ciowe, kt�re s� u�ywane przez kontroler
- Osobi�cie nie jestem fanem u�ywania tego samoego w kontrolerze i serwisie jako wej�cia np. `CreateProductForm`
- Patrz�c na projekt wydaje mi si�, �e nale�a�oby go sformatowa� po "funkcjonalno�ciach" i tak porozdziela� po folderach:
	- Orders
	- Products
	- Carts (GuestSessionService, CartService)
	- Authentication (TokenService, UserService)
	- Notification (EmailService, EmailSender)
- To samo zrobi� z repozytoriami.
- ...
