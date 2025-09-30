// Функция замены аватара
function replaceImgSrc(imgBlock, user) {
    console.log('⚠️ imgBlock: '+ imgBlock + '\nuser: ' + user);
    if (imgBlock && user) {
      // Получаем изображение на которое заменяем.
      const newImageUrl = (typeof browser !== 'undefined' ? browser : chrome).runtime.getURL('my-avatar.png');
      
      imgBlock.setAttribute('data-original-src', imgBlock.src); // сохраняем оригинал
      imgBlock.src = newImageUrl;
      
      imgBlock.addEventListener('click', function() { // вещаем листенер для возврата изображения по нажатию
        const originalSrc = this.getAttribute('data-original-src');
        if (originalSrc) {
          this.src = originalSrc;
          this.removeAttribute('data-original-src');
        }
      });
      
      console.log('✅ Аватар заменён!');
    }
}

// Нужно обновление через сервис интеграции.
function updateUsers() {
  // Если текущий пользователь на странице "Сотрудники", то обновить список сотрудников?
  if (window.location.href.includes("drx.rosa-it.ru/drxweb/#/list/b7905516-2be5-4931-961c-cb38d5677565")) {
    console.log("Отправить список сотрудников на сервер.");
    const employees = document.querySelectorAll('.columns-builder__text-container.columns-builder__text-container_linesmode_single[href]');
    employees.forEach(block => {
        if (block.querySelector('[href]') != null) {
            const span = block.querySelector('span');
            console.log(span.textContent);
        }
    });
  }
}

// Основной функционал, общий обработчик.
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
  
  // Аватар профиля в переписке.
  const avatarTaskBlocks = document.querySelectorAll('.thread-item-header')
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

// Вещаем обработчик только на выбранные блоки, чтобы лишний раз не нагружать.
const avatarSelectors = ['.current-user-info.header-view__button', '.popup popup_with-floating-panel', '.thread-item-header', '.thread-item-header__avatar-container', '.notice-thread-item__avatar-container', '.image-editor'];

// Также следим за изменениями DOM (на случай SPA)
const observer = new MutationObserver((mutations) => {
  // Вынести на действие по кнопке и через сервис интеграции.
  updateUsers()
  
  for (const mutation of mutations) {
    if (mutation.type === 'childList') {
      for (const node of mutation.addedNodes) {
        if (node.nodeType === Node.ELEMENT_NODE) {
          // Проверяем: подходит ли сам узел под один из селекторов?
          const matchesSelf = avatarSelectors.some(selector => node.matches(selector));

          // Или есть ли внутри него элемент, подходящий под один из селекторов?
          const matchesChild = avatarSelectors.some(selector => node.querySelector(selector));

          if (matchesSelf || matchesChild) {
            replaceAvatar();
            return; // достаточно одного совпадения
          }
        }
      }
    }
  }
});

observer.observe(document.body, { childList: true, subtree: true });