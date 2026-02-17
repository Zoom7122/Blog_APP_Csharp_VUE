<template>
  <div class="article-view">
    <div class="search-container">
      <input v-model="properties.Title" placeholder="Введите название">
      <input v-model="tagInput" placeholder="Введите тег">
      <button type="button" @click="FindByProperties">Найти по свойствам</button>
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
            <span class="date" v-if="article.PublishedAt">
              {{ formatDate(article.PublishedAt) }}
            </span>
                <div class="tags" v-if="article.Tags.length">
              <span class="tag" v-for="(tag, tagIndex) in article.Tags" :key="`${article.Id}-tag-${tagIndex}`">
                #{{ tag }}
              </span>
            </div>
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

        <button
          v-if="isAdmin"
          type="button"
          class="closeCommentsButton"
          @click="deleteArticle(article.Id)"
        >
          Удалить статью
        </button>

        <AddComment :articleId="article.Id"></AddComment>

        <button v-show="!showComments"
          @click="showComments = true"
          class="showCommentsButton">Показать комментарии к статье</button>

        <div v-show="showComments" class="divComments">
        <div class="countComments"> Колличество камментариев: {{ article.comments.length }}</div>

         <div v-for="(com, comIndex) in article.comments" :key="com.Id || comIndex">
          <div class="Comments">
            <p>Комментарий : {{ comIndex + 1 }}</p>
            <h5 class="nameAuthorConnent"> {{ com.UserName }}</h5>
            <p class="contentComments">{{ com.Content }}</p>
            <p class="timeComments"> Опубликован в {{ com.CreatedAt || 'Дата неизвестна'}}</p>
              <button
              v-if="isAdmin"
              type="button"
              class="closeCommentsButton"
              @click="deleteComment(article.Id, com.Id)"
            >
              Удалить комментарий
            </button>
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
    props: {
    userRole: {
      type: String,
      default: ''
    }
  },
  emits: ['close', 'logout'],
  data() {
    return {
      loading: false,
      ArticleList: [],
      tagInput: '',
      showComments: false,

      properties: {
        Title: "",
         Tags: []
      }
    }
  },
    computed: {
    isAdmin() {
      return this.userRole === 'Admin';
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
    async deleteArticle(articleId) {
      if (!articleId) return;
      try {
        await api.delete(`/Articles/${articleId}`);
        this.ArticleList = this.ArticleList.filter(x => x.Id !== articleId);
      } catch (err) {
        console.log('Ошибка удаления статьи: ', err);
      }
    },

    async deleteComment(articleId, commentId) {
      if (!commentId) return;
      try {
        await api.delete(`/Comments/${commentId}`);
        const article = this.ArticleList.find(x => x.Id === articleId);
        if (!article) return;

        article.comments = article.comments.filter(x => x.Id !== commentId);
      } catch (err) {
        console.log('Ошибка удаления комментария: ', err);
      }
    },

    
    async FindByProperties() {
      try {
        const normalizedTag = this.tagInput.trim();
        this.properties.Tags = normalizedTag ? [normalizedTag] : [];
        const response = await api.post('/Articles/FindByProperties', this.properties)

        if (response.data.success && response.data.list) {


          this.ArticleList = response.data.list.map(item => {
                const articleTags = Array.isArray(item.tags)
                  ? item.tags
                  : (item.tag
                    ? item.tag
                        .split(',')
                        .map(tag => tag.trim())
                        .filter(Boolean)
                    : []);
                return{
                Id: item.id || '',
                Title: item.title || '',
                Text: item.text || '',
                Tags: articleTags,
                Description: item.description || '',
                Cover_image: item.cover_image || '',
                Author_Email: item.author_Email || '',
                ReadTime: item.readTime || 0,
                PublishedAt: item.publishedAt || null,
                comments: item.comments ? item.comments.map(com => ({
                  Id: com.id || '',
                  UserName: com.userName || 'Нет имени',
                  Content: com.content || 'Нет текста',
                  CreatedAt: com.createdAt ? com.createdAt.slice(0, 10) : ''
                })) : []
            }
          });
          
        } else {
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