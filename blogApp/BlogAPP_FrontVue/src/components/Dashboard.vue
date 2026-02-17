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
        
        <p v-if="user.bio"><strong>О себе:</strong> {{ user.bio }}</p>

        <button class="edit-toggle-btn" @click="toggleEditProfile">
          {{ isEditingProfile ? 'Отменить редактирование' : 'Редактировать профиль' }}
        </button>

        <form v-if="isEditingProfile" class="edit-profile-form" @submit.prevent="updateProfile">
          <input v-model.trim="profileForm.firstName" type="text" minlength="2" required placeholder="Ваше имя" />
          <input v-model.trim="profileForm.email" type="email" required placeholder="Email" />
          <input v-model.trim="profileForm.avatar_url" type="url" placeholder="URL аватара" />
          <textarea v-model.trim="profileForm.bio" rows="3" placeholder="Коротко о себе"></textarea>

          <button type="submit" class="add-article-btn" :disabled="isUpdatingProfile">
            {{ isUpdatingProfile ? 'Сохраняем...' : 'Сохранить профиль' }}
          </button>
        </form>

        <p v-if="profileMessage" class="profile-message">{{ profileMessage }}</p>
        <p v-if="profileError" class="profile-error">{{ profileError }}</p>

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


<ArticleView v-show="showArticleView" :userRole="user.role"></ArticleView>
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
      isEditingProfile: false,
      isUpdatingProfile: false,
      profileMessage: '',
      profileError: '',
      profileForm: {
        firstName: '',
        email: '',
        avatar_url: '',
        bio: ''
      },
      features: [
        { id: 1, title: 'Создание статей', description: 'Пишите и публикуйте свои статьи' },
        { id: 2, title: 'Комментирование', description: 'Комментируйте статьи других пользователей' },
        { id: 3, title: 'Редактирование профиля', description: 'Настройте свой профиль' }
      ]
    };
  },
  methods: {
        toggleEditProfile() {
      this.isEditingProfile = !this.isEditingProfile;
      this.profileMessage = '';
      this.profileError = '';

      if (this.isEditingProfile) {
        this.profileForm = {
          firstName: this.user.name || '',
          email: this.user.email || '',
          avatar_url: this.user.avatar_url || '',
          bio: this.user.bio || ''
        };
      }
    },

    async updateProfile() {
      this.profileMessage = '';
      this.profileError = '';
      this.isUpdatingProfile = true;

      try {
        const response = await api.put('/Entrance/UpdateUser', {
          firstName: this.profileForm.firstName,
          email: this.profileForm.email,
          avatar_url: this.profileForm.avatar_url,
          bio: this.profileForm.bio
        });

        if (!response.data.success) {
          this.profileError = response.data.errorMessage || 'Не удалось обновить профиль';
          return;
        }

        const updatedUser = response.data.user || {};

        this.user.name = updatedUser.name || this.profileForm.firstName;
        this.user.email = updatedUser.email || this.profileForm.email;
        this.user.avatar_url = updatedUser.avatar_url || this.profileForm.avatar_url;
        this.user.bio = updatedUser.bio || this.profileForm.bio;
        this.user.role = updatedUser.role || this.user.role;
        this.user.countPost = updatedUser.countPost ?? this.user.countPost;

        this.profileMessage = 'Профиль успешно обновлен';
        this.isEditingProfile = false;
        this.$emit('profile-updated', { ...this.user });
      } catch (error) {
        this.profileError = error.response?.data?.errorMessage || 'Ошибка при обновлении профиля';
      } finally {
        this.isUpdatingProfile = false;
      }
    },
    async logout() {
        try {
            await api.post('/Entrance/Logout');
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
        const response = await api.get('/Comments/GetCountComments')

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
          const countP = await api.get('/Articles/GetCountArticle');

          this.user.countPost = countP.data

        } catch (error) {
        console.error('Ошибка при загрузке комментариев:', error);
    }
  },
  },
  async mounted() {
    console.log('Dashboard получил пользователя:', this.user);
      this.profileForm = {
      firstName: this.user.name || '',
      email: this.user.email || '',
      avatar_url: this.user.avatar_url || '',
      bio: this.user.bio || ''
    };
    this.GetCountComments();
    this.GetCountPost();
  }
}
</script>

<style scoped>
@import '@/components/css/Dashboard.css'
</style>