<template>
 <div class="dashboard" v-show="!showArticleForm && !showArticleView">
    <!-- Шапка Dashboard -->
    <div class="dashboard-header">
      <h1>Добро пожаловать в Dashboard!</h1>
      <button @click="logout" class="logout-btn">Выйти</button>
    </div>
    
    <!-- Информация о пользователе -->
    <div class="user-profile">
      <div class="avatar-section">
        <img 
          :src="user.avatar_url || '/default-avatar.png'" 
          :alt="user.name"
          class="avatar"
        />
      </div>
      
      <div class="user-info">
        <h2>{{ user.name }}</h2>
        <p><strong>Email:</strong> {{ user.email }}</p>
        <p><strong>Роль:</strong> {{ user.role }}</p>
        
        <div class="user-stats">
          <div class="stat">
            <span class="stat-number">{{user.countPost}}</span>
            <span class="stat-label">Статей</span>
          </div>
          <div class="stat">
            <span class="stat-number">{{ user.CountCommetsUser }}</span>
            <span class="stat-label">Комментариев</span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Основной контент Dashboard -->
    <div class="dashboard-content">
      <h3>Ваши возможности:</h3>
      <div class="features">
        <div class="feature-card" v-for="feature in features" :key="feature.id">
          <h4>{{ feature.title }}</h4>
          <p>{{ feature.description }}</p>
        </div>
      </div>
    </div>
    <div class="dashboard-content">

      <h3>Ваши возможности:</h3>
      <button @click="showArticleForm = true" class="add-article-btn">📝 Добавить статью</button>
      <button @click="showArticleView = true" class="add-article-btn">Найти статью</button>
    </div>
  </div>


<ArticleView v-show="showArticleView"></ArticleView>
<ArticleAdd v-show="showArticleForm" 
  @close="showArticleForm = false"
  @closeAddArticle="showArticleIsAdd"
></ArticleAdd>

</template>

<script>
import ArticleAdd from './ArticleAdd.vue';
import ArticleView from './ArticleView.vue';
import api from '@/axios-config';


export default {
  components: { ArticleAdd, ArticleView},
  name: 'Dashboard',
  props: {
    // Принимаем user из App.vue
    user: {
      type: Object,
      required: true,
      default: () => ({
        name: '',
        email: '',
        avatar_url: '',
        bio: '',
        role: '',
        countPost : 0,
        CountCommetsUser: 0,
      })
    }
  },
  data() {
    return {
      showArticleForm: false,
      showArticleView: false,
      features: [
        { id: 1, title: 'Создание статей', description: 'Пишите и публикуйте свои статьи' },
        { id: 2, title: 'Комментирование', description: 'Комментируйте статьи других пользователей' },
        { id: 3, title: 'Редактирование профиля', description: 'Настройте свой профиль' }
      ]
    };
  },
  methods: {
    async logout() {
        try {
            await api.post('/EntranceConroller/Logout');
        } catch (error) {
            console.error('Ошибка при выходе:', error);
        }
        this.$emit('logout');
    },

    showArticleIsAdd(data){
      console.log("Данные статьи:", JSON.stringify(data, null, 2)); 
      this.showArticleForm = false
    },

    async GetCountComments(){

      try{
        const response = await api.get('/Commets/GetCountComments')

        if(response.data.countCommets >0){
          this.user.CountCommetsUser = response.data.countCommets
        }
        else{
          this.user.CountCommetsUser =0
          console.log(response.data.errorMessege)
        }

      }
      catch(error) {

      }
    },
      async GetCountPost(){

     try {
          const countP = await api.get('ArticleConrtroller/GetCountArticle');

          this.user.countPost = countP.data

        } catch (error) {
        console.error('Ошибка при загрузке комментариев:', error);
    }
  },
  },
  async mounted() {
    console.log('Dashboard получил пользователя:', this.user);
    this.GetCountComments();
    this.GetCountPost();
  }
}
</script>

<style scoped>
@import '@/components/css/Dashboard.css'
</style>