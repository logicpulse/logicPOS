using LogicPOS.UI.Buttons;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace LogicPOS.UI.Components.Common.Buttons
{
    public class ButtonImageProcessor
    {
        private readonly ConcurrentQueue<Item> _queue = new ConcurrentQueue<Item>();
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        public ButtonImageProcessor()
        {
            _worker.WorkerSupportsCancellation = false;
            _worker.DoWork += Process;
            _worker.RunWorkerAsync();
        }

        private void Process(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (_queue.TryDequeue(out Item item)) {

                    string imagePath = item.ImageGetter.Invoke();
                    if (imagePath != null)
                    {
                        item.Button.UpdateImage(imagePath);
                    }
                }
            }
        }

        public void ProcessButtonImage(ImageButton button, Func<string> func)
        {
            var item = new Item { Button = button, ImageGetter = func };
            _queue.Enqueue(item);
        }

        private struct Item
        {
            public ImageButton Button { get; set; }
            public Func<string> ImageGetter { get; set; }
        }
    }
}
