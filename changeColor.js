document.getElementById('changeColor').addEventListener('click', () => {
  chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
    chrome.scripting.executeScript({
      target: { tabId: tabs[0].id },
      func: changePageColor
    });
  });
});

function changePageColor() {
  document.body.style.backgroundColor =
    '#' + Math.floor(Math.random()*16777215).toString(16);
}
