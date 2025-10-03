const API_URL = "http://localhost:12001/api"; // адрес бэкенда

// Отправка списка сотрудников
async function sendUsersToServer(users) {
  const res = await fetch(`${API_URL}/users/batch`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(users)
  });

  if (!res.ok) {
    throw new Error("Ошибка при сохранении сотрудников");
  }
  return await res.json(); // например вернуть список ID сотрудников
}

// Получение аватара с сервера
async function getGeneratedAvatar(displayName) {
  const res = await fetch(`${API_URL}/users/by-displayname/${encodeURIComponent(displayName)}`);
  if (!res.ok) return null;
  const data = await res.json();
  return data.generatedImageUrl;
}

// Модифицируем replaceImgSrc чтобы брать с сервера
async function replaceImgSrc(imgBlock, user) {
  console.log('⚠️ replaceImgSrc. imgBlock: '+ imgBlock + '\nuser: ' + user);
  if (imgBlock && user) {
    const newImageUrl = await getGeneratedAvatar(user);
    if (!newImageUrl) return;
    
    console.log('⚠️ getGeneratedAvatar. generatedImageUrl: '+ newImageUrl);
    

    imgBlock.setAttribute('data-original-src', imgBlock.src);
    imgBlock.src = newImageUrl;

    imgBlock.addEventListener('click', function () {
      const originalSrc = this.getAttribute('data-original-src');
      if (originalSrc) {
        this.src = originalSrc;
        this.removeAttribute('data-original-src');
      }
    });

    console.log(`✅ Аватар ${user} заменён на ИИ-версию`);
  }
}

// Обновление пользователей со страницы
async function updateUsers() {
  if (window.location.href.includes("drx.rosa-it.ru/drxweb/#/list")) {
    const employees = [];
    document.querySelectorAll('.columns-builder__text-container.columns-builder__text-container_linesmode_single[href]').forEach(block => {
      const span = block.querySelector('span');
      if (span) {
        employees.push({ displayName: span.textContent });
      }
    });

    if (employees.length > 0) {
      try {
        const res = await sendUsersToServer(employees); // ждём ответ
        console.log("✅ Список сотрудников отправлен", res);
      } catch (err) {
        console.error("❌ Ошибка при отправке сотрудников:", err);
      }
    }
  }
}

// Основной функционал, общий обработчик.
function replaceAvatar() {
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
  
  chrome.storage.local.set({ currentUser: currentUser });
  
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