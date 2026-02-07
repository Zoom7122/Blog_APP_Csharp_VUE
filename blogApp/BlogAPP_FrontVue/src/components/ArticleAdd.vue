<template>
  <div class="article-create-container" @submit.prevent="SendArticle">
    <div class="header">
      <h1>Новая статья</h1>
      <p>Заполните все поля для публикации статьи</p>
    <button @click="CloseAddView" class="btn btn-primary">
        Выйти
    </button>
    </div>


    <form class="article-form">
      <!-- Заголовок -->
      <div class="form-group">
        <label for="title">Заголовок статьи *</label>
        <input
          type="text"
          id="title"
          v-model="article.Title"
          placeholder="Введите заголовок..."
          required
          class="form-input"
        />
      </div>

      <!-- Slug (ЧПУ) -->
      <div class="form-group">
        <label for="slug">URL адрес (slug)</label>
        <input
          type="text"
          id="slug"
          v-model="article.CoverImage"
          placeholder="nazvanie-stati"
          class="form-input"
        />
        <small class="hint">Будет использоваться в ссылке на статью</small>
      </div>

      <!-- Краткое описание -->
      <div class="form-group">
        <label for="excerpt">Краткое описание *</label>
        <textarea
          id="excerpt"
          v-model="article.description"
          placeholder="Краткое описание статьи (будет показано в списках)"
          rows="3"
          class="form-textarea"
        ></textarea>
      </div>

      <!-- Основной контент -->
      <div class="form-group">
        <label for="content">Текст статьи *</label>
        <textarea
          id="content"
          v-model="article.text"
          placeholder="Начните писать вашу статью здесь..."
          rows="10"
          class="form-textarea large"
        ></textarea>
      </div>

      <!-- Мета-данные -->
      <div class="meta-section">
        <h3>Мета-данные</h3>
        <div class="meta-cards">
          <div class="meta-card">
            <span class="meta-label">Время чтения</span>
            <div class="meta-input-group">
              <input
                type="number"
                v-model="article.ReadTime"
                min="0"
                class="meta-input"
                placeholder="5"
              />
              <span class="meta-unit">мин.</span>
            </div>
          </div>
        </div>
      </div>

      <div v-if="errorApi!=''" class="error-box">
      <strong>Ошибка:</strong> {{ errorApi }}
     </div>

     <p v-if="successSendData == true">Успешно</p>

      <!-- Кнопки действий -->
      <div class="form-actions">
        <buttonsubmit type="submit" class="btn btn-primary">
            Сохранить
        </buttonsubmit>
      </div>
    </form>
  </div>
</template>

<script>
import api from '@/axios-config';

export default {
  name: 'ArticleCreate',
  data() {
    return {
      article: {
        Title: '',
        CoverImage: '',
        description: '',
        text: '',
        ReadTime: 0
      },
        errorApi: false,
        successSendData: false
    }
  },
  methods: {
    CloseAddView(){
      this.$emit('close');
    },
    handleSaveDraft() {
      this.article.status = 'draft'
      console.log('Сохранение черновика:', this.article)
      // Логика сохранения черновика
    },
    handleImageError(event) {
      event.target.style.display = 'none'
      console.log('Ошибка загрузки изображения')
    },
    async SendArticle() {
      this.errorApi = '';

       console.log('Отправляю данные на C# API:', this.article)
      try{
      const response = await api.post('/ArticleConrtroller/CreateArticle', this.article);

      if(response.data.success == true){
        this.successSendData = true
      }
      else {
        this.errorApi = "Ошибка: сервер вернул false";
      }
    }
    catch(err)
    {
      const mes = err.response?.data?.messegeError || err.message;
      this.errorApi = "Ошибка отправки: " + mes;
    }
    }
  },
  mounted() {
    // Устанавливаем текущую дату и время по умолчанию
    const now = new Date()
    const localDateTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
      .toISOString()
      .slice(0, 16)
    this.article.published_at = localDateTime
  }
}
</script>

<style scoped>
.article-create-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  background-color: #fff;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.header {
  display: flex;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #f0f0f0;
    align-items: center; /* Вертикальное выравнивание */
  justify-content: space-between; /* h1 слева, кнопка справа */
  padding: 20px;
  position: relative; /* Для абсолютного позиционирования кнопки если нужно */
}

.header h1 {
  color: #333;
  margin: 0 0 10px 0;
  font-size: 28px;
}

.header p {
  color: #666;
  margin: 0;
  font-size: 16px;
}

.article-form {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #444;
  font-size: 15px;
}

.form-group label[required]::after {
  content: " *";
  color: #ff4757;
}

.form-input,
.form-textarea,
.form-select {
  padding: 12px 15px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  transition: all 0.3s;
  background-color: #fff;
}

.form-input:focus,
.form-textarea:focus,
.form-select:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 3px rgba(66, 184, 131, 0.1);
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
  line-height: 1.5;
}

.form-textarea.large {
  min-height: 200px;
}

.hint {
  color: #888;
  font-size: 13px;
  margin-top: 4px;
}

.image-input-wrapper {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.image-preview {
  width: 100%;
  max-width: 300px;
  border-radius: 8px;
  overflow: hidden;
  border: 2px solid #e0e0e0;
}

.image-preview img {
  width: 100%;
  height: 150px;
  object-fit: cover;
  display: block;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
    gap: 25px;
  }
}

.meta-section {
  padding: 20px;
  background-color: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.meta-section h3 {
  margin: 0 0 15px 0;
  color: #495057;
  font-size: 18px;
}

.meta-cards {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.meta-card {
  flex: 1;
  min-width: 150px;
  padding: 15px;
  background: white;
  border-radius: 6px;
  border: 1px solid #dee2e6;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.meta-label {
  font-size: 13px;
  color: #6c757d;
  margin-bottom: 5px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.meta-value {
  font-size: 24px;
  font-weight: 700;
  color: #42b883;
}

.meta-input-group {
  display: flex;
  align-items: center;
  gap: 8px;
}

.meta-input {
  width: 80px;
  padding: 8px 10px;
  border: 2px solid #e0e0e0;
  border-radius: 4px;
  text-align: center;
  font-size: 18px;
  font-weight: 600;
}

.meta-unit {
  color: #6c757d;
  font-size: 14px;
}

.form-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-end;
  padding-top: 20px;
  border-top: 2px solid #f0f0f0;
}

.btn {
  padding: 12px 24px;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  min-width: 160px;
}

.btn-primary {
  background-color: #42b883;
  color: white;
}

.btn-primary:hover {
  background-color: #3aa876;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(66, 184, 131, 0.3);
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #5a6268;
}
</style>

