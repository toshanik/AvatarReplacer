// Функция замены аватара
function replaceImgSrc(img, user) {
    console.log('⚠️ img: '+ img + '\nuser: ' + user);
    if (img && user) {
      const newImageUrl = (typeof browser !== 'undefined' ? browser : chrome).runtime.getURL('my-avatar.png');
      img.src = newImageUrl;
      console.log('✅ Аватар заменён!');
    }
}
// Функция замены аватара
function replaceAvatar() {
  var userName = '';
  // Фото текущего пользователя.
  const currentUserBlock = document.querySelector('.current-user-info.header-view__button');
  const currentUser = currentUserBlock ? currentUserBlock.title : null;
  if (currentUserBlock == null || currentUser == null) {
      console.log('⚠️ currentUserBlock: '+ currentUserBlock + '\ncurrentUser: ' + currentUser);
      return;
  }
  
  userImg = currentUserBlock.querySelector('img');
  replaceImgSrc(userImg, currentUser);
  
  console.log('Текущий пользователь: ' + currentUser);
  
  // Всплывающее окно.
  const popupBlocks = document.querySelectorAll('.popup popup_with-floating-panel');
  popupBlocks.forEach(block => {
    const img = block.querySelector('img');
    const user = document.querySelectorAll('.user-digest__header link');
    replaceImgSrc(img, user.textContent);
  });
  
  // Аватар профиля в заголовках переписки.
  const avatarTaskBlocks = document.querySelectorAll('.thread-item-header')
  // Аватар профиля в переписке.
  const avatarAssigmentBlocks = document.querySelectorAll('.thread-item-header__avatar-container'); //.popup-digest-container
  const avatarNoticeBlocks = document.querySelectorAll('.notice-thread-item__avatar-container'); //.popup-digest-container
  const avatarBlocks = [...avatarTaskBlocks, ...avatarAssigmentBlocks, ...avatarNoticeBlocks, ];
  
  avatarBlocks.forEach(block => {
    const img = block.querySelector('img');
    replaceImgSrc(img, img.alt);
  });
  
  // Фото сотрудника.
  const photoBlocks = document.querySelectorAll('.image-editor')
  const userNameBlock = document.querySelector('.context-title__title-text')
  photoBlocks.forEach(block => {
    const img = block.querySelector('img');
    replaceImgSrc(img, userNameBlock.title);
  });
}

// Запускаем сразу
replaceAvatar();

// Также следим за изменениями DOM (на случай SPA)
const observer = new MutationObserver(replaceAvatar);
observer.observe(document.body, { childList: true, subtree: true });