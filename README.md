![thumbnail](assets/thumbnail.png)

Instead of usual boring vulture masks, this mod makes them more individualized.

New individualized masks consist of multiple parts: eye cutouts, lower mask, forehead, horns, side horns. (Paint is not yet done.)

The ID of vulture and/or mask entity determines the pieces of which the new mask will consist of.

The mod then chooses the pieces and bakes them into a texture, 9 times for each rotation.

This also works for king vulture masks, but not for elite scavengers or scavenger king.

My weird design choices:
- some sprites are duplicated to make the mask variety closer to vanilla.
- the texture is merged into one pixel by pixel, and all 9 rotations *are not* stored as one atlas, but as *separate atlases of one rotation*. This is because i don't know how atlases work in the game.
- technically CWT should make sure there is no remaining masks left, but *i do not remember* if the mask sprites get deleted when the mask is gone/vulture dies for memory. Most likely not.
- the sprites are dumped into StreamingAssets during playtime as .pngs for debugging purposes.
- the scavenger paint was supposed to be implemented, for elite scavengers, but i do not know how to do it. Maybe like king vulture arrows idk.

TODO:
- split the code by modules
- stop the debugging output of textures into streamingassets
- custom scavenger paint maybe someday idk


# AddressBook Assistant Bot

## 🇺🇦 Опис

CLI бот-асистент для управління контактами, днями народження, адресами, email та нотатками.

# 🔧 Функціонал:

- Додавання, редагування, видалення контактів
- Збереження номерів телефонів з валідацією міжнародного формату
- Додавання та перегляд email з валідацією
- Додавання адрес (країна, місто, вулиця, будинок, квартира)
- Збереження дня народження та перегляд ДН у межах заданих дат
- Нотатки: створення, редагування, видалення, пошук, теги, сортування
- Серіалізація даних (pickle)
- Кольоровий вивід (colorama)
- Красиві таблиці (rich)
- Структура SRC + запуск через pyproject.toml

# Доступні команди:

| Command          | Aliases             | Description                           | Usage                               |
|------------------|---------------------|---------------------------------------|-------------------------------------|
| **Контакти**     |                     |                                       |                                     |
| add              | create              | Додати новий контакт та телефон       | add <ім'я> <телефон>                |
| all              |                     | Показати всі контакти                 | all                                 |
| delete-contact   | remove              | Видалити контакт                      | delete-contact <ім'я>               |
| find             |                     | Знайти контакт                        | find <ключове_слово>                |
| **Контакти - Адреса**|                 |                                       |                                     |
| add-address      |                     | Додати адресу до контакту             | add-address <ім'я> <адреса>         |
| addresses        |                     | Показати адреси                       | addresses <ім'я>                    |
| change-address   |                     | Змінити адресу                        | change-address <ім'я> <...>         |
| delete-address   |                     | Видалити адресу                       | delete-address <ім'я>               |
| **Контакти - День Народження**|        |                                       |                                     |
| add-birthday     |                     | Додати день народження                | add-birthday <ім'я> <ДД.ММ.РРРР>    |
| birthdays        |                     | Показати дні народження найближчі N днів | birthdays <дні>                  |
| change-birthday  |                     | Змінити день народження               | change-birthday <ім'я> <ДД.ММ.РРРР> |
| delete-birthday  |                     | Видалити день народження              | delete-birthday <ім'я>              |
| **Контакти - Пошта**|                  |                                       |                                     |
| add-email        |                     | Додати email                          | add-email <ім'я> <email>            |
| change-email     |                     | Змінити email                         | change-email <ім'я> <новий_email>   |
| delete-email     |                     | Видалити email                        | delete-email <ім'я>                 |
| emails           |                     | Показати email                        | emails <ім'я>                       |
| **Контакти - Ім'я**|                   |                                       |                                     |
| change-name      |                     | Змінити ім'я контакту                 | change-name <старе_ім'я> <нове_ім'я>|
| **Контакти - Телефон**|                |                                       |                                     |
| change           |                     | Змінити існуючий номер телефону       | change <ім'я> <старий_телефон> <новий_телефон>|
| phone            |                     | Показати номери телефону контакту     | phone <ім'я>                        |
| **Замітки**      |                     |                                       |                                     |
| add-note         |                     | Додати нотатку                        | add-note <заголовок> <вміст>        |
| all-note         |                     | Показати всі нотатки                  | all-note                            |
| delete-note      |                     | Видалити нотатку                      | delete-note <заголовок>             |
| **Замітки - Зміст**|                   |                                       |                                     |
| change-note      |                     | Змінити вміст нотатки                 | change-note <заголовок> <новий_вміст>|
| find-note        |                     | Знайти нотатку                        | find-note <ключове_слово>           |
| **Замітки - Теги**|                    |                                       |                                     |
| add-tag          |                     | Додати тег до нотатки                 | add-tag <заголовок> <тег>           |
| delete-tag       |                     | Видалити тег                          | delete-tag <заголовок> <тег>        |
| edit-tag         |                     | Змінити тег                           | edit-tag <заголовок> <старий_тег> <новий_тег>|
| find-tag         |                     | Знайти нотатки за тегом               | find-tag <тег>                      |
| **Замітки - Заголовок**|               |                                       |                                     |
| change-title     |                     | Змінити заголовок нотатки             | change-title <старий_заголовок> <новий_заголовок>|
| **Інше**         | bye, close, quit    | Вийти з бота                          | exit                                |
| hello            |                     | Привітати користувача                 | hello                               |
| help             |                     | Показати доступні команди             | help                                |


# 🚀 **Встановлення та запуск**

1. Клонувати репозиторій:

- git clone https://github.com/andriy-h80/addressbook-assistant-bot-final
- cd addressbook-assistant-bot-final

2. Встановити залежності:

- pip install -e .

3. Запуск бота:

Windows:

- python -m src.main

Mac/Linux:

- python3 -m src.main

## GB Description

# CLI assistant bot for managing contacts, birthdays, addresses, emails, and notes.

# 🔧 Features:
- Add, edit, delete contacts
- Phone validation (international format)
- Email validation
- Add addresses (country, city, street, building, apartment)
- Save birthdays and view upcoming birthdays within date range
- Notes: create, edit, delete, search, tags, sort
- Data persistence with pickle serialization
- Colorful CLI output (colorama)
- Rich (rich)
- SRC project structure + pyproject.toml for easy installation

# Available Commands

| Command          | Aliases             | Description                     | Usage                               |
|------------------|---------------------|---------------------------------|-------------------------------------|
| **Contacts**     |                     |                                 |                                     |
| add              | create              | Add new contact and phone       | add <name> <phone>                  |
| all              |                     | Show all contacts               | all                                 |
| delete-contact   | remove              | Delete Contact                  | delete-contact <name>               |
| find             |                     | Find contact                    | find <field> <keyword>              |
| **Contacts - Address**|                |                                 |                                     |
| add-address      |                     | Add address to contact          | add-address <name> <address>        |
| addresses        |                     | Show addresses                  | addresses <name>                    |
| change-address   |                     | Change address                  | change-address <name> <...>         |
| delete-address   |                     | Delete address                  | delete-address <name>               |
| **Contacts - Birthday**|               |                                 |                                     |
| add-birthday     |                     | Add birthday                    | add-birthday <name> <DD.MM.YYYY>    |
| birthdays        |                     | Show birthdays in next N days   | birthdays <days>                    |
| change-birthday  |                     | Change birthday                 | change-birthday <name> <DD.MM.YYYY> |
| delete-birthday  |                     | Delete birthday                 | delete-birthday <name>              |
| **Contacts - Email**|                  |                                 |                                     |
| add-email        |                     | Add email                       | add-email <name> <email>            |
| change-email     |                     | Change email                    | change-email <name> <new_email>     |
| delete-email     |                     | Delete email                    | delete-email <name>                 |
| emails           |                     | Show emails                     | emails <name>                       |
| **Contacts - Name**|                   |                                 |                                     |
| change-name      |                     | Change contact's name           | change-name <old_name> <new_name>   |
| **Contacts - Phone**|                  |                                 |                                     |
| change           |                     | Change existing phone number    | change <name> <old_phone> <new_phone>|
| phone            |                     | Show phone numbers for contact  | phone <name>                        |
| **Notes**        |                     |                                 |                                     |
| add-note         |                     | Add note                        | add-note <title> <content>          |
| all-note         |                     | Show all notes                  | all-note                            |
| delete-note      |                     | Delete note                     | delete-note <title>                 |
| **Notes - Content**|                   |                                 |                                     |
| change-note      |                     | Change note content             | change-note <title> <new_content>   |
| find-note        |                     | Find note                       | find-note <keyword>                 |
| **Notes - Tags** |                     |                                 |                                     |
| add-tag          |                     | Add tag to note                 | add-tag <title> <tag>               |
| delete-tag       |                     | Delete a tag                    | delete-tag <title> <tag>            |
| edit-tag         |                     | Edit a tag                      | edit-tag <title> <old_tag> <new_tag>|
| find-tag         |                     | Find notes by tag               | find-tag <tag>                      |
| **Notes - Title**|                     |                                 |                                     |
| change-title     |                     | Change note title               | change-title <old_title> <new_title>|
| **Util**         | bye, close, quit    | Exit bot                        | exit                                |
| hello            |                     | Greets the user                 | hello                               |
| help             |                     | Show available commands         | help                                |


# 🚀 Installation

1. Clone the repository:

- git clone https://github.com/andriy-h80/addressbook-assistant-bot-final
- cd addressbook-assistant-bot-final

2. Install dependencies:

- pip install -e .

3. Run the assistant bot:

Windows:

- python -m src.main

Mac/Linux:

- python3 -m src.main

# 👥 Authors

- Roman
- Andriy
- Maksym

# 📄 License

- This project is licensed under the MIT License.


