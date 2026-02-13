<template>
  <div class="article-create-container">
    <div class="header">
      <h1>Новая статья</h1>
      <p>Заполните все поля для публикации статьи</p>
    <button @click="CloseAddView" class="btn btn-primary" style="background-color: #ff5252;">
        Выйти
    </button>
    </div>


    <form class="article-form"  @submit.prevent="SendArticle">
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

      <div class="form-group">
        <label for="title">Теги статьи</label>
        <div v-for="(tag, index) in article.Tag" :key="index" class="tag-chip">
           {{ tag }}
           <span @click="removeTag(index)" class="remove-tag">&times;</span>
        </div> 
        <input
          type="text"
          v-model="tagInput"
          @keydown.enter.prevent="addTag"
          placeholder="Введите тег и нажмите Enter..."
          class="form-input tag-input"
        />
         <small class="hint">Нажмите Enter, чтобы добавить тег</small>
      </div>

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
          v-model="article.Description"
          placeholder="Краткое описание статьи (будет показано в списках)"
          rows="3"
          required
          class="form-textarea"
        ></textarea>
      </div>

      <div class="form-group">
        <label for="content">Текст статьи *</label>
        <textarea
          id="content"
          v-model="article.Text"
          placeholder="Начните писать вашу статью здесь..."
          rows="10"
          required
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
                min="1"
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
      <button type="submit" class="btn btn-primary">
          Сохранить
      </button>
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
        Description: '',
        Text: '',
        ReadTime: 1,
        Tag: []
      },
        tagInput: '',
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
    },
    handleImageError(event) {
      event.target.style.display = 'none'
      console.log('Ошибка загрузки изображения')
    },
    async SendArticle() {
      this.errorApi = '';
      
        console.log('1. Начинаю отправку...');
        console.log('2. Данные для отправки:', JSON.stringify(this.article));
       
       console.log('Отправляю данные на C# API:', this.article)
      try{
      const response = await api.post('/ArticleConrtroller/CreateArticle', this.article);

      if(response.data.success == true){
        this.successSendData = true

        this.$emit('closeAddArticle', this.article);
        
        this.article = {
          Title: '',
          CoverImage: '',
          Description: '',
          Text: '',
          ReadTime: 1,
          Tag: []
        }
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
    },
    addTag(){
      const val =this.tagInput.trim()
      if(val && !this.article.Tag.includes(val)){
        this.article.Tag.push(val)
      }
      this.tagInput ='';
    },
    removeTag(index) {
      this.article.Tag.splice(index, 1);
    },
  },
  mounted() {
    const now = new Date()
    const localDateTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
      .toISOString()
      .slice(0, 16)
    this.article.published_at = localDateTime
  }
}
</script>

<style scoped>
@import '@/components/css/ArticleAdd.css'
</style>
