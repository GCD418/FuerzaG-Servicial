document.addEventListener('DOMContentLoaded', function() {
    const dropdownContainer = document.getElementById('userDropdownContainer');
    const dropdownMenu = dropdownContainer?.querySelector('.dropdown-menu');
    
    if (dropdownContainer && dropdownMenu) {
        dropdownContainer.addEventListener('mouseenter', function() {
            dropdownMenu.classList.add('show');
            
            setTimeout(() => {
                const rect = dropdownMenu.getBoundingClientRect();
                const windowWidth = window.innerWidth;
                
                if (rect.right > windowWidth) {
                    dropdownMenu.style.left = 'auto';
                    dropdownMenu.style.right = '0';
                }
            }, 10);
        });
        
        dropdownContainer.addEventListener('mouseleave', function() {
            dropdownMenu.classList.remove('show');
        });
    }
});