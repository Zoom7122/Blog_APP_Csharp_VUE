# 🚀 BlogAPP — Fullstack Blog Platform
### ASP.NET Core 10 + Web API + MVC + Vue 3

[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)]()
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-blue)]()
[![Vue](https://img.shields.io/badge/Vue-3-42b883)]()
[![SQLite](https://img.shields.io/badge/Database-SQLite-lightgrey)]()
[![License](https://img.shields.io/badge/License-Educational-green)]()

Учебный fullstack-проект блога с многослойной архитектурой и **двумя клиентами**:
- ASP.NET Core MVC (серверный UI)
- Vue 3 + Vite (SPA)

```
Client (MVC или Vue) → API/Controllers → BLL → DAL → SQLite
```

---

## 📚 Содержание

- [Возможности](#-возможности)
- [Технологии](#-технологии)
- [Архитектура и проекты решения](#-архитектура-и-проекты-решения)
- [Требования](#-требования)
- [Быстрый запуск](#-быстрый-запуск)
  - [Вариант A: запуск API + Vue](#вариант-a-запуск-api--vue)
  - [Вариант B: запуск MVC](#вариант-b-запуск-mvc)
- [Аутентификация и авторизация](#-аутентификация-и-авторизация)
- [REST API endpoints](#-rest-api-endpoints)
- [База данных](#-база-данных)
- [Полезные команды](#-полезные-команды)
- [Возможные проблемы](#-возможные-проблемы)

---

## ✨ Возможности

- Многослойная архитектура: `API/MVC → BLL → DAL`
- Cookie-аутентификация
- Ролевая авторизация (`Admin`, `User`)
- CRUD для статей
- Комментарии к статьям
- Глобальная обработка ошибок (middleware)
- Swagger/OpenAPI для API
- Логирование в MVC через NLog

---

## 🛠 Технологии

### Backend
- .NET 10
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- SQLite
- Cookie Authentication
- AutoMapper
- Swagger / OpenAPI
- NLog (в MVC)

### Frontend
- Vue 3
- Vite
- Axios

---

## 🏗 Архитектура и проекты решения

Решение: `blogApp/BlogAPP.slnx`

| Проект | Назначение |
|---|---|
| `BlogAPP_API` | REST API (контроллеры, CORS, cookie auth, swagger) |
| `BlagAPP_MVC` | MVC-клиент (представления, фильтры, middleware, логирование) |
| `BlogAPP_BLL` | Бизнес-логика |
| `BlogAPP_DAL` | Репозитории + `DbContext` |
| `BlogAPP_Core` | DTO/модели общего назначения |
| `BlogAPP_FrontVue` | Vue SPA |

---

## ⚙ Требования

- .NET SDK **10.0**
- Node.js **20.19+** или **22.12+**
- npm

---

## 🚀 Быстрый запуск

### Вариант A: запуск API + Vue

1) Backend API:

```bash
cd blogApp/BlogAPP_API
dotnet restore
dotnet run
```

По умолчанию (Development):
- HTTPS: `https://localhost:7284`
- HTTP: `http://localhost:5277`
- Swagger: `https://localhost:7284/swagger`

2) Frontend Vue (в отдельном терминале):

```bash
cd blogApp/BlogAPP_FrontVue
npm install
npm run dev -- --host
```

Vue dev server:
- `http://localhost:5173`

> В API настроен CORS для `http://localhost:5173`.

---

### Вариант B: запуск MVC

```bash
cd blogApp/BlagAPP_MVC
dotnet restore
dotnet run
```

По умолчанию (Development):
- HTTPS: `https://localhost:7105`
- HTTP: `http://localhost:5257`

Стартовая точка перенаправляет на `/Login`.

---

## 🔐 Аутентификация и авторизация

Используется Cookie Authentication:
- API cookie: `AuthCookie`
- MVC cookie: `MvcAuthCookie`

Ограничения по ролям:
- Удаление статей и комментариев требует роль `Admin`.

Основные auth-маршруты API:

| Метод | Endpoint | Описание |
|---|---|---|
| POST | `/api/Entrance/Login` | Вход |
| POST | `/api/Entrance/Register` | Регистрация |
| POST | `/api/Entrance/Logout` | Выход |
| GET | `/api/Entrance/CheckAuth` | Проверка сессии |
| PUT | `/api/Entrance/UpdateUser` | Обновление профиля |

---

## 📡 REST API endpoints

### Articles (`/api/Articles`)

| Метод | Endpoint | Описание |
|---|---|---|
| POST | `/CreateArticle` | Создание статьи |
| POST | `/FindByProperties` | Поиск статей |
| GET | `/GetCountArticle` | Кол-во статей пользователя |
| DELETE | `/{articleId}` | Удаление статьи (`Admin`) |

### Comments (`/api/Comments`)

| Метод | Endpoint | Описание |
|---|---|---|
| POST | `/CreateComments` | Создание комментария |
| GET | `/GetCountComments` | Кол-во комментариев пользователя |
| DELETE | `/{commentId}` | Удаление комментария (`Admin`) |

---

## 🗄 База данных

- Тип БД: SQLite
- Файл БД в репозитории: `blogApp/BlogAPP_DAL/db/db.db`
- Контекст EF Core: `Blog_DBcontext`

Основные сущности:
- `User`
- `Article`
- `Comment`
- `Tag`
- `Reaction`
- `Article_Tag`

---

## 📦 Полезные команды

### .NET (из `blogApp/`)

```bash
dotnet restore BlogAPP.slnx
dotnet build BlogAPP.slnx
```

### Vue (из `blogApp/BlogAPP_FrontVue`)

```bash
npm run dev -- --host
npm run build
npm run preview
```

---

## ⚠ Возможные проблемы

### 1) Cookie не сохраняется между фронтом и API
- Используйте корректные URL/протоколы (HTTP/HTTPS).
- Проверьте, что backend запущен и доступен.

### 2) CORS ошибка во Vue
- Убедитесь, что API запущен.
- Проверьте, что фронтенд открыт на `http://localhost:5173`.

### 3) Ошибка сборки .NET
- Проверьте, что установлен именно .NET SDK 10.0.

---

## 🎓 Назначение

Проект создан в учебных целях для практики:
- многослойной архитектуры,
- REST API,
- cookie-аутентификации,
- интеграции ASP.NET Core и Vue.

---

## 📌 Автор

Влад Любченко (@Zoom71222)
