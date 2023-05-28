## Про архитектуру

Есть два сервиса - AuthService и RestaurantService. Они используют общую базу данных.
Для этого ТЗ мне показалось излишним иметь две БД - это был бы скорее оверхед по ресурсам (железу),
если бы мы разворачивали эти сервисы куда-то в прод. Сервис авторизации использует всего одну табличку, никаких сложных запросов не делает, поэтому паразитную нагрузку на cpu оказывать не будет, соответственно мешать основному сервису не должен.

Таблица `session`, указанная в ТЗ, не добавлялась в схему БД. Данные, которые должны были храниться в этой таблице, вполне себе вмещает в себя сам JWT-токен. Во-первых, мы для каждого запроса валидируем переданный токен - в этом нам помогает секретный ключ, которым мы подписываем токен при генерации. Соответственно, токены которые мы никому не выдавали - принимать не будем. Во-вторых, токен в себе уже содержит информацию о экспирации, хранить ее где-то дополнительно не нужно. То же самое и про идентификацию пользователя - просто складываем в токен его электронную почту, например. Так что таблица `session` не нужна, не хотим лишний раз делать запросики в базу.

Из других расхождений с ТЗ - в табличку с юзерами добавлена колонка PasswordSalt. Ну, это вообще довольно стандартная тема, когда речь касается хэширования паролей - странно что в ТЗ по умолчанию этого нет. Хэшированные без соли пароли вполне можно обознать, например [вот так](https://ru.wikipedia.org/wiki/%D0%A0%D0%B0%D0%B4%D1%83%D0%B6%D0%BD%D0%B0%D1%8F_%D1%82%D0%B0%D0%B1%D0%BB%D0%B8%D1%86%D0%B0). Особенно если пароль легкий, а пользователи довольно часто пренебрегают безопасностью своих паролей. А вот нагенерировав рандомную соль - эти проблемы уходят.

В остальном все сделано по ТЗ.

## Как локально запускать

Два варианта.

Запустить в докере только постгрес, а сами приложения запустить локально на своей машине. Будут слушать два каких-то порта. Так, конечно, можно, но зачем? Я так делал для разработки, чтобы не ждать каждый раз, когда образ приложений сбилдится, когда вношу изменения какие-то. А вот когда нужно запустить потыкать - рекомендую вариант два.

Вариант два заключается в том, чтобы имитировать продакшен-окружение и все приложения запустить в докере. Для этого уже все готово - и образы описаны, и конфигурация запуска описана в докер-композе. В подарок вы получаете карманный api-gateway в качестве nginx. Он по запрашиваемому пути определит, в какой сервис проксировать запрос. nginx, кстати, наружу пробросит 80 порт. Так что запускайте в докере, кидайте запросы на локалхост, и радуйтесь жизни без страшных четырехзначных портов!

Запускать вот так:
```
docker-compose -f .\docker-compose.postgres.yaml -f .\docker-compose.production.yaml up 
```

## Про обработку заказов

В RestaurantService есть бэкграунд таска, которые каждые 15 секунд достает из базы заказы со статусами Queued и InProgress и просто проставляет им следующий в лайфсайкле заказа статус. Ну то есть, если Queued, то следующим станет InProgress, и соответственно после InProgress следует Finished.

Так что после создания заказа можно потыкать эндпоинт получения статуса заказа и увидеть, как он станет сначала InProgress, а через некоторое время - Finished.