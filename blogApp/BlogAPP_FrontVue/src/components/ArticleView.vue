<template>
  <div class="article-view">
    <div class="search-container">
      <input v-model="properties.Title" placeholder="Введите название">
      <input v-model="properties.Tag" placeholder="Введите тег">
      <button @click="FindByProperties()">Найти по свойствам</button>
    </div>

    <!-- Сообщение если статьи не найдены -->
    <div v-if="ArticleList.length === 0 && !loading" class="no-articles">
      Статьи не найдены. Попробуйте изменить критерии поиска.
    </div>

    <!-- Отображение списка статей -->
    <div v-if="ArticleList.length > 0">
      <div class="articles-count">Найдено статей: {{ ArticleList.length }}</div>
      
      <div v-for="(article, index) in ArticleList" :key="index" class="article-item">
        <div class="article-header">
          <h2 class="article-title">{{ article.Title || 'Без названия' }}</h2>
          <div class="article-meta">
            <span class="author">Автор: {{ article.Author_Name || 'Неизвестный автор' }}</span>
            <span class="date" v-if="article.PublishedAt">
              {{ formatDate(article.PublishedAt) }}
            </span>
            <span class="tag" v-if="article.Tag">#{{ article.Tag }}</span>
            <span class="read-time" v-if="article.ReadTime">Время чтения: {{ article.ReadTime }} мин.</span>
          </div>
        </div>
        
        <div class="article-description" v-if="article.Description">
          {{ article.Description }}
        </div>
        
        <div class="article-content">
          <p>{{ article.Text || 'Содержание статьи отсутствует' }}</p>
        </div>
        
        <div class="article-footer">
          <span class="email" v-if="article.Author_Email">Email автора: {{ article.Author_Email }}</span>
        </div>

        <AddComment :articleId="article.Id"></AddComment>
        
        <button v-show="!showComments" 
        @click="showComments = true"
        class="showCommentsButton" >Показать комментарии к статье</button>

        <div v-show="showComments" class="divComments">
        <div class="countComments"> Колличество камментариев: {{ article.comments.length }}</div>

        <div v-for="(com, comIndex) in article.comments">
          <div class="Comments">
            <p>Комментарий : {{ comIndex + 1 }}</p>
            <h5 class="nameAuthorConnent"> {{ com.UserName }}</h5>
            <p class="contentComments">{{ com.Content }}</p>
            <p class="timeComments"> Опубликован в {{ com.CreatedAt || "1212"}}</p>
          </div>
        </div>

        <button v-show="showComments" 
          @click="showComments = false"
          class="closeCommentsButton">Скрыть комментарии к статье</button>

        </div>
        
        <hr v-if="index < ArticleList.length - 1">
      </div>
    </div>

    <!-- Индикатор загрузки -->
    <div v-if="loading" class="loading">
      Загрузка статей...
    </div>
  </div>
</template>


<script>
import AddComment from './AddComment.vue';
import api from '@/axios-config';

export default {
  components:{AddComment},
  data() {
    return {
      ArticleList: [],
      
      showComments: false,

      properties: {
        Title: null,
        Tag: null
      }
    }
  },
  
  methods: {
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString)
      return date.toLocaleDateString('ru-RU', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    },
    
    async FindByProperties() {
      try {
        console.log('Пользователь ввел: ' + this.properties.Title)
        const response = await api.post('ArticleConrtroller/FindByProperties', this.properties)

        console.log('Ответ от API: ', response.data)
        console.log('Успех: ' + response.data.success)

        if (response.data.success && response.data.list) {
          
          if (response.data.list.length > 0) {
            
            console.log('Полученные данные: ', response.data.list)

          }

          this.ArticleList = response.data.list.map(item => {
            const article = {
                Id: item.id || '',
                Title: item.title || '',
                Text: item.text || '',
                Tag: item.tag || '',
                Description: item.description || '',
                Cover_image: item.cover_image || '',
                Author_Name: item.author_Name || '',
                Author_Email: item.author_Email || '',
                ReadTime: item.readTime || 0,
                PublishedAt: item.publishedAt || null,
                comments: item.comments ? item.comments.map(com => ({
                  UserName: com.userName || 'Нет имени',
                  Content: com.content || 'Нет текста',
                  CreatedAt: com.createdAt.slice(0,10) || ''
                })) : []
            }
           console.log('Преобразованный элемент: ', article);

            console.log('Структура полученного элемента:', Object.keys(item));
            return article;
          });
          
          console.log('Массив статей: ', this.ArticleList)
        } else {
          console.log('Статьи не найдены: ' + (response.data.messegeEror || response.data.messageError || ''))
          this.ArticleList = []
        }
      }
      catch(err) {
        console.log('Ошибка Api: ', err)
        this.ArticleList = []
      }
    }
  }
}
</script>

<style scoped>
@import '@/components/css/ArticleView.css';
</style>