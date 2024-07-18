using System.Linq;

namespace Collections
{
    public class Trie<T> : ITrie<T>
    {
        public class Node<U>
        {
            public U Value { get; set; }
            public System.UInt32 FlagCount { get; set; }
            public System.Collections.Generic.IDictionary<U, Node<U>> Children { get; set; }

            public Node (U value, System.UInt32 flagCount)
            {
                this.Value = value;
                this.FlagCount = flagCount;
                this.Children = new System.Collections.Generic.Dictionary<U, Node<U>>(128);
                return;
            }
        }

        private Node<T> Root { get; set; }

        public Trie()
        {
            this.Root = new Node<T>(default, System.UInt32.MaxValue);
            return;
        }
        public Trie(System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<T>> collection) : this()
        {
            foreach (var element in collection)
            {
                this.Insert(element);
            }
            return;
        }

        public void Insert(System.Collections.Generic.IEnumerable<T> data)
        {
            if (data is null) throw new System.ArgumentNullException(nameof(data), "This data is null");

            var current = this.Root;
            foreach (T element in data)
            {
                if (!current.Children.ContainsKey(element))
                {
                    current.Children.Add (element, new Node<T> (element, 0));
                }
                current = current.Children[element];
            }
            if (current != this.Root) current.FlagCount++;
            return;
        }

        public System.Boolean Remove(System.Collections.Generic.IEnumerable<T> target)
        {
            if (target is null) throw new System.ArgumentNullException(nameof(target), "This target is null");
            else if (target.Count<T>() is 0) throw new System.ArgumentException("This target count is 0", nameof(target));

            var previous = new System.Collections.Generic.Stack<Node<T>>();
            if (previous is null) throw new System.NullReferenceException("The dynamic memory allocation failed..");

            previous.Push(this.Root);

            var current = this.Root;
            foreach (var element in target)
            {
                if (!current.Children.ContainsKey(element)) return false;

                current = current.Children[element];
                previous.Push(current);
            }
            if (previous.Peek()?.FlagCount == 0) return false;

            while (previous.Count != 0)
            {
                var deleteNode = previous.Pop();
                if (deleteNode.Children.Count != 0)
                {
                    deleteNode.FlagCount = 0;
                    return true;
                }
                if (previous.Count < 1) return true;

                var parentNode = previous.Peek();
                parentNode.Children.Remove(deleteNode.Value);
            }
            return true;
        }

        public System.Boolean DecreaseFlagCount(System.Collections.Generic.IEnumerable<T> target)
        {
            if (target is null) throw new System.ArgumentNullException(nameof(target), "This target is null");

            var previous = new System.Collections.Generic.Stack<Node<T>>();
            if (previous is null) throw new System.NullReferenceException("The dynamic memory allocation failed..");

            previous.Push(this.Root);

            var current = this.Root;
            foreach (var element in target)
            {
                if (!current.Children.ContainsKey(element)) return false;

                current = current.Children[element];
                previous.Push(current);
            }
            if (previous.Peek()?.FlagCount == 0) return false;

            while (previous.Count > 1)
            {
                var deleteNode = previous.Pop();
                if (deleteNode.FlagCount > 1)
                {
                    deleteNode.FlagCount--;
                    return true;
                }
                if (deleteNode.Children.Count != 0)
                {
                    deleteNode.FlagCount = 0;
                    return true;
                }
                if (previous.Count < 1) return true;

                var parentNode = previous.Peek();
                parentNode.Children.Remove(deleteNode.Value);
            }
            return true;
        }

        public System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<T>> Search(System.Collections.Generic.IEnumerable<T> target)
        {
            if (target is null) throw new System.ArgumentNullException(nameof(target), "This target is null");

            var destination = new System.Collections.Generic.Stack<T>();
            if (destination is null) throw new System.NullReferenceException("The dynamic memory allocation failed..");

            var current = this.Root;
            foreach (var element in target)
            {
                if (!current.Children.ContainsKey(element)) yield break;
                current = current.Children[element];
                destination.Push(element);
            }
            destination.Pop();

            foreach (var result in this.FindAll(current, destination))
            {
                yield return result.destination;
            }
        }

        public System.Collections.Generic.IEnumerable<(System.Collections.Generic.IEnumerable<T> destination, System.UInt32 count)> Find(System.Collections.Generic.IEnumerable<T> target)
        {
            if (target is null) throw new System.ArgumentNullException(nameof(target), "This target is null");

            var destination = new System.Collections.Generic.Stack<T>();
            if (destination is null) throw new System.NullReferenceException("The dynamic memory allocation failed..");

            var current = this.Root;
            foreach (var element in target)
            {
                if (!current.Children.ContainsKey(element)) yield break;
                current = current.Children[element];
                destination.Push(element);
            }
            destination.Pop();

            foreach (var result in this.FindAll(current, destination))
            {
                yield return result;
            }
        }

        private System.Collections.Generic.IEnumerable<(System.Collections.Generic.IEnumerable<T> destination, System.UInt32 count)> FindAll (Node<T> current, System.Collections.Generic.Stack<T> destination)
        {
            destination.Push(current.Value);
            if (current.FlagCount > 0 && current.FlagCount != System.UInt32.MaxValue) yield return (new System.Collections.Generic.Stack<T>(destination) ?? throw new System.NullReferenceException("The dynamic memory allocation failed..."), current.FlagCount);

            foreach (var child in current.Children)
            {
                foreach (var result in this.FindAll (child.Value, destination))
                {
                    yield return result;
                }
                destination.Pop();
            }            
        }

        public System.Collections.Generic.IEnumerable<(System.Collections.Generic.IEnumerable<T> destination, System.UInt32 count)> FindAll () => this.FindAll (this.Root, new System.Collections.Generic.Stack<T>());
        
        private System.Collections.Generic.IEnumerable<Node<T>> Traversal (Node<T> node)
        {
            var buffer = new System.Collections.Generic.Stack<Node<T>>();
            buffer.Push(node);

            while (buffer.Count > 0)
            {
                var current = buffer.Pop();
                yield return current;

                foreach (var child in current.Children)
                {
                    buffer.Push(child.Value);
                }
            }
        }

        public void Clear()
        {
            var buffer = new System.Collections.Generic.Stack<Node<T>>(this.Traversal(this.Root));
            foreach (var child in buffer)
            {
                child.Children.Clear();
            }
            return;
        }

        public void Dispose()
        {
            this.Clear();
            this.Root = null;
            return;
        }
    }
}
