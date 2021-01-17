function readStream()
{
    const evtSource = new EventSource('/Counter/Stream');
    evtSource.onmessage = function(event) {
        const el = document.getElementById('counter');
        const { value } = JSON.parse(event.data);
        
        el.textContent = value;
    }
}

function startCounter()
{
    fetch('/Counter/Start', { method: 'PUT' });
}

function stopCounter()
{
    fetch('/Counter/Stop', { method: 'PUT' });
}

function setCounter()
{
    async function putData(url = '', data = {}) {
      const response = await fetch(url, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        referrerPolicy: 'no-referrer',
        body: JSON.stringify(data)
      });

      // parses JSON response into native JavaScript objects
      return response.json();
    }
    
    const el = document.getElementById('set-value');
    const value = Number(el.value);

    putData('/Counter', { value });
}
